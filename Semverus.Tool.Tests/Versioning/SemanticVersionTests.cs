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
		Assert.Throws<IdentifierNotValidException>(() => new SemanticVersion(1, 2, 3, expectedPrerelease));
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
		Assert.Throws<IdentifierNotValidException>(() => new SemanticVersion(1, 2, 3, metadata: expectedMetadata));
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
