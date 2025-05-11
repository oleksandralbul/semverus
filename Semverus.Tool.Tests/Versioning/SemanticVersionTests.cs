using Semverus.Tool.Versioning;

namespace Semverus.Tool.Tests.Versioning;

public class SemanticVersionTests
{
	#region " Constructors tests"

	[Fact]
	public void SemanticVersion_Major_VersionCorrect()
	{
		var expectedMajor = 1;

		var semver = new SemanticVersion(expectedMajor);

		semver.Verify(expectedMajor);
	}

	[Fact]
	public void SemanticVersion_NegativeMajor_ThrowArgumentOutOfRangeException()
	{
		var major = -1;

		Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(major));
	}

	[Fact]
	public void SemanticVersion_MajorMinor_VersionCorrect()
	{
		var expectedMinor = 1;

		var semver = new SemanticVersion(0, expectedMinor);

		semver.Verify(minor: expectedMinor);
	}

	[Fact]
	public void SemanticVersion_NegativeMinor_ThrowArgumentOutOfRangeException()
	{
		var minor = -1;

		Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(0, minor));
	}

	[Fact]
	public void SemanticVersion_MajorMinorPatch_VersionCorrect()
	{
		var expectedPatch = 1;

		var semver = new SemanticVersion(0, 0, expectedPatch);

		semver.Verify(patch: expectedPatch);
	}

	[Fact]
	public void SemanticVersion_NegativePatch_ThrowArgumentOutOfRangeException()
	{
		var patch = -1;

		Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(0, 0, patch));
	}

	[Theory]
	[InlineData("alpha")]
	[InlineData("ALPHA")]
	[InlineData("alpha.1")]
	[InlineData("alpha.2-3")]
	[InlineData("alpha.1.2.3")]
	[InlineData("1.2.3")]
	[InlineData("1.-.1")]
	public void SemanticVersion_PrereleaseLabel_VersionCorrect(string expectedPrerelease)
	{
		var semver = new SemanticVersion(1, 2, 3, expectedPrerelease);

		semver.Verify(prerelease: expectedPrerelease);
	}

	[Theory]
	[InlineData("!@#$%")]
	[InlineData("alpha..1")]
	[InlineData(".")]
	public void SemanticVersion_IncorrectPrereleaseLabel_Throws(string expectedPrerelease)
	{
		Assert.Throws<ArgumentException>(() => new SemanticVersion(1, 2, 3, expectedPrerelease));
	}

	[Theory]
	[InlineData("alpha")]
	[InlineData("ALPHA")]
	[InlineData("alpha.1")]
	[InlineData("alpha.2-3")]
	[InlineData("alpha.1.2.3")]
	[InlineData("1.2.3")]
	[InlineData("1.-.1")]
	public void SemanticVersion_MetadataLabel_VersionCorrect(string expectedMetadata)
	{
		var semver = new SemanticVersion(1, 2, 3, metadata: expectedMetadata);

		semver.Verify(metadata: expectedMetadata);
	}

	[Theory]
	[InlineData("!@#$%")]
	[InlineData("alpha..1")]
	[InlineData(".")]
	public void SemanticVersion_IncorrectMetadataLabel_Throws(string expectedMetadata)
	{
		Assert.Throws<ArgumentException>(() => new SemanticVersion(1, 2, 3, metadata: expectedMetadata));
	}

	#endregion " Constructors tests"

	#region " ICloneable tests "

	[Fact]
	public void Clone_ReturnObjectIsEqual()
	{
		var expected = new SemanticVersion(1, 2, 3, "alpha.5", "My.Super.Metadata");

		var actual = expected.Clone();

		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ICloneable_Clone_ReturnObjectIsEqual()
	{
		ICloneable expected = new SemanticVersion(1, 2, 3, "alpha.5", "My.Super.Metadata");

		var actual = expected.Clone();

		Assert.Equal(expected, actual);
	}

	#endregion " ICloneable tests "

	#region " IComparable tests "

	[Fact]
	public void CompareToDifferentObject_ShouldThrowArgumentException()
	{
		var semver = new SemanticVersion(1, 2, 3);

		Assert.Throws<ArgumentException>(() => semver.CompareTo(42));
	}

	[Fact]
	public void CompareToNull_ShouldReturn1()
	{
		const int expectedCompareResult = 1;
		var semver = new SemanticVersion(1, 2, 3);

		var actualCompareResult = semver.CompareTo(null);
		Assert.Equal(expectedCompareResult, actualCompareResult);
	}

	[Theory]
	[InlineData(1, 2, -1)]
	[InlineData(2, 1, 1)]
	[InlineData(1, 1, 0)]
	public void CompareMajorVersions_ExpectCorrectResult(int leftMajor, int rightMajor, int expectedCompareResult)
	{
		var left = new SemanticVersion(leftMajor);
		var right = new SemanticVersion(rightMajor);

		var actualCompareResult = left.CompareTo(right);

		Assert.Equal(expectedCompareResult, actualCompareResult);
	}

	[Theory]
	[InlineData(1, 2, -1)]
	[InlineData(2, 1, 1)]
	[InlineData(1, 1, 0)]
	public void CompareMinorVersions_ExpectCorrectResult(int leftMinor, int rightMinor, int expectedCompareResult)
	{
		const int major = 1;

		var left = new SemanticVersion(major, leftMinor);
		var right = new SemanticVersion(major, rightMinor);

		var actualCompareResult = left.CompareTo(right);

		Assert.Equal(expectedCompareResult, actualCompareResult);
	}

	[Theory]
	[InlineData(1, 2, -1)]
	[InlineData(2, 1, 1)]
	[InlineData(1, 1, 0)]
	public void ComparePatchVersions_ExpectCorrectResult(int leftPatch, int rightPatch, int expectedCompareResult)
	{
		const int major = 1, minor = 2;

		var left = new SemanticVersion(major, minor, leftPatch);
		var right = new SemanticVersion(major, minor, rightPatch);

		var actualCompareResult = left.CompareTo(right);

		Assert.Equal(expectedCompareResult, actualCompareResult);
	}

	[Theory]
	[InlineData(null, null, 0)]
	[InlineData("alpha.1", null, -1)]
	[InlineData(null, "alpha.1", 1)]
	[InlineData("alpha.2", "alpha.1", 1)]
	[InlineData("alpha.1", "alpha.2", -1)]
	[InlineData("alpha", "alpha", 0)]
	[InlineData("alpha", "beta", -1)]
	[InlineData("alpha.beta", "beta.alpha", -1)]
	[InlineData("alpha.1.2", "alpha.1", 1)]
	[InlineData("alpha", "123", 1)]
	[InlineData("alpha", "-123456", 1)]
	public void CompareTwoPrereleaseVersion_ExpectCorrectResult(string? leftPrerelease, string? rightPrerelease, int expectedCompareResult)
	{
		const int major = 1, minor = 2, patch = 3;

		var left = new SemanticVersion(major, minor, patch, leftPrerelease);
		var right = new SemanticVersion(major, minor, patch, rightPrerelease);

		var actualCompareResult = left.CompareTo(right);

		Assert.Equal(expectedCompareResult, actualCompareResult);
	}

	#endregion " IComparable tests "

	#region " IFormattable and ToString() methods tests "

	[Theory]
	[InlineData("1.2.3", "1.2.3")]
	[InlineData("1.2.3+metadata", "1.2.3")]
	[InlineData("1.2.3-alpha.1", "1.2.3-alpha.1")]
	[InlineData("1.2.3-alpha.1+metadata", "1.2.3-alpha.1")]
	public void ToString_ShouldReturnNormalizedString(string version, string expected)
		=> Assert.Equal(SemanticVersion.Parse(version).ToString(), expected);

	[Theory]
	[InlineData("1.2.3", "1.2.3")]
	[InlineData("1.2.3+metadata", "1.2.3")]
	[InlineData("1.2.3-alpha.1", "1.2.3-alpha.1")]
	[InlineData("1.2.3-alpha.1+metadata", "1.2.3-alpha.1")]
	public void ToNormalizedString_ShouldReturnNormalizedString(string version, string expected)
		=> Assert.Equal(SemanticVersion.Parse(version).ToNormalizedString(), expected);

	[Theory]
	[InlineData("1.2.3")]
	[InlineData("1.2.3+metadata")]
	[InlineData("1.2.3-alpha.1")]
	[InlineData("1.2.3-alpha.1+metadata")]
	public void ToFullString_ShouldReturnFullVersion(string version)
		=> Assert.Equal(SemanticVersion.Parse(version).ToFullString(), version);

	[Theory]
	[InlineData("1.2.3", "N", "1.2.3")]
	[InlineData("1.2.3+metadata", "N", "1.2.3")]
	[InlineData("1.2.3-alpha.1", "N", "1.2.3-alpha.1")]
	[InlineData("1.2.3-alpha.1+metadata", "N", "1.2.3-alpha.1")]
	[InlineData("1.2.3", "F", "1.2.3")]
	[InlineData("1.2.3+metadata", "F", "1.2.3+metadata")]
	[InlineData("1.2.3-alpha.1", "F", "1.2.3-alpha.1")]
	[InlineData("1.2.3-alpha.1+metadata", "F", "1.2.3-alpha.1+metadata")]
	[InlineData("1.2.3", "V", "1.2.3")]
	[InlineData("1.2.3+metadata", "V", "1.2.3")]
	[InlineData("1.2.3-alpha.1", "V", "1.2.3")]
	[InlineData("1.2.3-alpha.1+metadata", "V", "1.2.3")]
	[InlineData("1.2.3", "R", "")]
	[InlineData("1.2.3+metadata", "R", "")]
	[InlineData("1.2.3-alpha.1", "R", "alpha.1")]
	[InlineData("1.2.3-alpha.1+metadata", "R", "alpha.1")]
	[InlineData("1.2.3", "M", "")]
	[InlineData("1.2.3+metadata", "M", "metadata")]
	[InlineData("1.2.3-alpha.1", "M", "")]
	[InlineData("1.2.3-alpha.1+metadata", "M", "metadata")]
	[InlineData("1.2.3", "x", "1")]
	[InlineData("1.2.3+metadata", "x", "1")]
	[InlineData("1.2.3-alpha.1", "x", "1")]
	[InlineData("1.2.3-alpha.1+metadata", "x", "1")]
	[InlineData("1.2.3", "y", "2")]
	[InlineData("1.2.3+metadata", "y", "2")]
	[InlineData("1.2.3-alpha.1", "y", "2")]
	[InlineData("1.2.3-alpha.1+metadata", "y", "2")]
	[InlineData("1.2.3", "z", "3")]
	[InlineData("1.2.3+metadata", "z", "3")]
	[InlineData("1.2.3-alpha.1", "z", "3")]
	[InlineData("1.2.3-alpha.1+metadata", "z", "3")]
	[InlineData("1.2.3-alpha.1+metadata", "x.y.z-R+M", "1.2.3-alpha.1+metadata")]
	[InlineData("1.2.3", "x.y", "1.2")]
	[InlineData("1.2.3", "x.y.z", "1.2.3")]
	[InlineData("1.2.3", "xyz", "123")]
	[InlineData("1.2.3", "test", "test")]
	[InlineData("1.2.3", "x.y.z (test)", "1.2.3 (test)")]
	[InlineData("1.2.3", "vx.y.z", "v1.2.3")]
	[InlineData("1.2.3-alpha.5+metadata", "vx.y.zVFN", "v1.2.31.2.31.2.3-alpha.5+metadata1.2.3-alpha.5")]
	public void ToString_UsingProvider_ShouldReturnCorrectOutput(string version, string format, string expected)
		=> Assert.Equal(expected, SemanticVersion.Parse(version).ToString(format, null));

	#endregion " IFormattable and ToString() methods tests "

	#region " IEquatable tests "

	[Fact]
	public void EqualsWithNull_ReturnFalse()
	{
		var semver = new SemanticVersion(1, 2, 3);
		Assert.False(semver.Equals(null));
	}

	[Fact]
	public void EqualsWithOtherObject_ReturnFalse()
	{
		var semver = new SemanticVersion(1, 2, 3);
		Assert.False(semver.Equals(42));
	}

	[Fact]
	public void EqualsTwoDifferentVersions_ReturnFalse()
	{
		var semver = new SemanticVersion(1, 2, 3);
		var otherSemver = new SemanticVersion(1, 2, 4);

		Assert.False(semver.Equals(otherSemver));
	}

	[Fact]
	public void EqualsSameObject_ReturnTrue()
	{
		var semver = new SemanticVersion(1, 2, 3);
		var otherSemver = semver;

		Assert.True(semver.Equals(otherSemver));
	}

	[Fact]
	public void EqualsTwoEqualVersions_ReturnTrue()
	{
		var semver = new SemanticVersion(1, 2, 3);
		var otherSemver = semver.Clone();

		Assert.True(semver.Equals(otherSemver));
	}

	#endregion " IEquatable tests "

	#region " IParsable tests "

	[Theory]
	[InlineData("1.2.3", 1, 2, 3)]
	[InlineData("235.142.57864", 235, 142, 57864)]
	[InlineData("1.2.3-alpha", 1, 2, 3, "alpha")]
	[InlineData("1.2.3-alpha-1", 1, 2, 3, "alpha-1")]
	[InlineData("1.2.3+alpha", 1, 2, 3, null, "alpha")]
	[InlineData("1.2.3-alpha.1+beta.2", 1, 2, 3, "alpha.1", "beta.2")]
	public void Parse_ShouldParseCorrect(
		string input,
		int major,
		int minor,
		int patch,
		string? prerelease = null,
		string? metadata = null)
	{
		var expectedSemver = new SemanticVersion(major, minor, patch, prerelease, metadata);

		var actualSemver = SemanticVersion.Parse(input);

		Assert.Equal(expectedSemver, actualSemver);
	}

	[Fact]
	public void ParseNull_ThrowsArgumentNullExcpetion()
		=> Assert.Throws<ArgumentNullException>(() => SemanticVersion.Parse(null!));

	[Theory]
	[InlineData("2147483648.1.1")]
	[InlineData("1.2147483648.1")]
	[InlineData("1.1.2147483648")]
	public void ParseOverflowVersionNumbers_ThrowsOverflowException(string input)
		=> Assert.Throws<OverflowException>(() => SemanticVersion.Parse(input));

	[Theory]
	[InlineData("")]
	[InlineData("1")]
	[InlineData("1.2")]
	[InlineData("1,2.3")]
	[InlineData("1.2,3")]
	[InlineData("a.2.3")]
	[InlineData("1.b.3")]
	[InlineData("1.2.c")]
	[InlineData("1a.2.3")]
	[InlineData("1.2b.3")]
	[InlineData("1.2.3c")]
	[InlineData("1.2.3-+")]
	[InlineData("1-beta.1")]
	[InlineData("1+meta.1")]
	[InlineData("1.2-beta.1")]
	[InlineData("1.2+meta.1")]
	[InlineData("1.2.3- +meta.2")]
	[InlineData("1.2.3+ meta")]
	public void ParseCorruptString_ThrowFormatException(string input)
		=> Assert.Throws<FormatException>(() => SemanticVersion.Parse(input));

	#endregion " IParsable tests "

	#region " Arithmetic operators tests "

	[Fact]
	public void EqualOperator_TwoNullsAreEqual()
	{
		SemanticVersion? left = null, right = null;
		Assert.True(left == right);
	}

	[Fact]
	public void EqualOperator_SameReference_ShouldBeEqual()
	{
		var left = new SemanticVersion(1, 2, 3);
		var right = left;

		Assert.Equal(left, right);
	}

	[Theory]
	[InlineData("1.2.3", "1.2.3")]
	[InlineData("1.2.3-alpha", "1.2.3-alpha")]
	[InlineData("1.2.3-alpha.1", "1.2.3-alpha.1")]
	[InlineData("1.2.3-alpha.beta", "1.2.3-alpha.beta")]
	[InlineData("1.2.3-123", "1.2.3-123")]
	[InlineData("1.2.3--a1", "1.2.3--a1")]
	[InlineData("1.2.3-alpha+build.42", "1.2.3-alpha+build.84")]
	public void EqualOperator_ValuesShouldBeEqual(string left, string right)
	{
		var leftSemver = SemanticVersion.Parse(left);
		var rightSemver = SemanticVersion.Parse(right);

		Assert.True(leftSemver == rightSemver);
	}

	[Theory]
	[InlineData("1.2.3", null)]
	[InlineData(null, "1.2.3")]
	[InlineData("1.0.0", "2.0.0")]
	[InlineData("1.0.0", "1.1.0")]
	[InlineData("1.0.0", "1.0.1")]
	[InlineData("1.0.0-alpha", "1.0.1")]
	[InlineData("1.0.0-alpha.1", "1.0.1-alpha")]
	[InlineData("1.0.0-123", "1.0.1-124")]
	public void NotEqualOperator_ValuesShouldNotBeEqual(string? left, string? right)
	{
		var leftSemver = left is null ? null : SemanticVersion.Parse(left);
		var rightSemver = right is null ? null : SemanticVersion.Parse(right);

		Assert.True(leftSemver != rightSemver);
	}

	[Theory]
	[InlineData(null, "1.0.0")]
	[InlineData("1.0.0", "1.0.1")]
	[InlineData("1.0.0", "1.1.0")]
	[InlineData("1.1.0-alpha", "1.1.0")]
	[InlineData("1.1.0-alpha", "1.1.0-beta")]
	[InlineData("1.1.0-alpha.1", "1.1.0-alpha.2")]
	public void LessOperator_LeftValueShouldBeLessThanRight(string? left, string right)
	{
		var leftSemver = left is null ? null : SemanticVersion.Parse(left);
		var rightSemver = SemanticVersion.Parse(right);

		Assert.True(leftSemver < rightSemver);
	}

	[Theory]
	[InlineData("1.0.0", null)]
	[InlineData("1.0.1", "1.0.0")]
	[InlineData("1.1.0", "1.0.0")]
	[InlineData("1.1.0", "1.1.0-alpha")]
	[InlineData("1.1.0-beta", "1.1.0-alpha")]
	[InlineData("1.1.0-alpha.2", "1.1.0-alpha.1")]
	public void GreaterOperator_LeftValueShouldBeGreaterThanRight(string left, string? right)
	{
		var leftSemver = SemanticVersion.Parse(left);
		var rightSemver = right is null ? null : SemanticVersion.Parse(right);

		Assert.True(leftSemver > rightSemver);
	}

	#endregion " Arithmetic operators tests "
}

internal static class SemanticVersionTestExtensions
{
	public static void Verify(this SemanticVersion version,
		int? major = null,
		int? minor = null,
		int? patch = null,
		string? prerelease = null,
		string? metadata = null)
	{
		if (major.HasValue) Assert.Equal(major, version.Major);
		if (minor.HasValue) Assert.Equal(minor, version.Minor);
		if (patch.HasValue) Assert.Equal(patch, version.Patch);
		if (prerelease != null) Assert.Equal(prerelease, version.Prerelease);
		if (metadata != null) Assert.Equal(metadata, version.Metadata);
	}
}
