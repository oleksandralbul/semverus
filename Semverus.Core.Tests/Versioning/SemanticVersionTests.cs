using Semverus.Core.Versioning;

namespace Semverus.Core.Tests.Versioning;

public class SemanticVersionTests
{
	#region " Constructors tests "

	[Fact]
	public void SemanticVersion_ShouldAssignMajorVersionNumber()
	{
		var expectedMajor = 1;

		var semver = new SemanticVersion(expectedMajor, 0, 0);

		Assert.Equal(expectedMajor, semver.Major);
	}

	[Fact]
	public void SemanticVersion_ShouldThrowExceptionIfMajorNegative()
	{
		var expectedMajor = -1;

		Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(expectedMajor, 0, 0));
	}

	[Fact]
	public void SemanticVersion_ShouldAssignMinorVersionNumber()
	{
		var expectedMinor = 1;

		var semver = new SemanticVersion(0, expectedMinor, 0);

		Assert.Equal(expectedMinor, semver.Minor);
	}

	[Fact]
	public void SemanticVersion_ShouldThrowExceptionIfMinorNegative()
	{
		var expectedMinor = -1;

		Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(0, expectedMinor, 0));
	}

	[Fact]
	public void SemanticVersion_ShouldAssignPatchVersionNumber()
	{
		var expectedPatch = 1;

		var semver = new SemanticVersion(0, 0, expectedPatch);

		Assert.Equal(expectedPatch, semver.Patch);
	}

	[Fact]
	public void SemanticVersion_ShouldThrowExceptionIfPatchNegative()
	{
		var expectedPatch = -1;

		Assert.Throws<ArgumentOutOfRangeException>(() => new SemanticVersion(0, 0, expectedPatch));
	}

	[Fact]
	public void SemanticVersion_PrereleaseShouldAcceptLettersDigitsAndDash()
	{
		var prerelease = GetIdentifierCharactersWithoutDot();

		var semver = new SemanticVersion(0, 0, 0, prerelease);

		Assert.Equal(prerelease, semver.Prerelease);
	}

	[Fact]
	public void SemanticVersion_PassPrerelease_HasPrereleaseShouldBeTrue()
	{
		var prerelease = GetIdentifierCharactersWithoutDot();

		var semver = new SemanticVersion(0, 0, 0, prerelease);

		Assert.True(semver.HasPrerelease);
	}

	[Fact]
	public void SemanticVersion_PrereleaseIsNull_HasPrereleaseShouldBeFalse()
	{
		var semver = new SemanticVersion(0, 0, 0, prerelease: null);

		Assert.False(semver.HasPrerelease);
	}

	[Fact]
	public void SemanticVersion_PrereleaseIsEmptyStringOrContainsInvalidCharacters_ThrowException()
	{
		Assert.Throws<ArgumentException>(() => new SemanticVersion(0, 0, 0, string.Empty));

		var invalidCharacters = GetNotIdentifierCharactersString();
		foreach (var c in invalidCharacters)
		{
			var prerelease = c.ToString();
			Assert.Throws<ArgumentException>(() => new SemanticVersion(0, 0, 0, prerelease));
		}
	}

	[Fact]
	public void SemanticVersion_PrereleaseShouldAcceptLettersDigitsAndDashSeparatedByDot()
	{
		var prerelease = GetIdentifierCharactersWithoutDot() + "." + GetIdentifierCharactersWithoutDot();

		var semver = new SemanticVersion(0, 0, 0, prerelease);

		Assert.Equal(prerelease, semver.Prerelease);
	}

	[Fact]
	public void SemanticVersion_PrereleaseShouldAcceptPrereleaseIdentifiers()
	{
		var prereleaseIdentifiers = new List<string>
		{
			GetIdentifierCharactersWithoutDot(),
			GetIdentifierCharactersWithoutDot()
		};

		var semver = new SemanticVersion(0, 0, 0, prereleaseIdentifiers);

		var expectedPrerelease = string.Join('.', prereleaseIdentifiers);
		Assert.Equal(expectedPrerelease, semver.Prerelease);
	}

	[Fact]
	public void SemanticVersion_PrereleaseIdentifiersIsEmpty_HasPrereleaseShouldBeFalse()
	{
		var semver = new SemanticVersion(0, 0, 0, []);

		Assert.False(semver.HasPrerelease);
	}

	[Fact]
	public void SemanticVersion_PrereleaseIdentifiersContainsInvalidCharacters_ThrowsException()
	{
		Assert.Throws<ArgumentException>(() => new SemanticVersion(0, 0, 0, [string.Empty]));

		var invalidCharacters = GetNotIdentifierCharactersString();
		foreach (var c in invalidCharacters)
			Assert.Throws<ArgumentException>(() => new SemanticVersion(0, 0, 0, [c.ToString()]));
	}

	[Fact]
	public void SemanticVersion_MetadataShouldAcceptLettersDigitsAndDash()
	{
		var metadata = GetIdentifierCharactersWithoutDot();

		var semver = new SemanticVersion(0, 0, 0, prerelease: null, metadata);

		Assert.Equal(metadata, semver.Metadata);
	}

	[Fact]
	public void SemanticVersion_PassMetadata_HasMetadataShouldBeTrue()
	{
		var metadata = GetIdentifierCharactersWithoutDot();

		var semver = new SemanticVersion(0, 0, 0, prerelease: null, metadata);

		Assert.True(semver.HasMetadata);
	}

	[Fact]
	public void SemanticVersion_MetadataIsNull_HasMetadataShouldBeFalse()
	{
		var semver = new SemanticVersion(0, 0, 0, prerelease: null, metadata: null);

		Assert.False(semver.HasMetadata);
	}

	[Fact]
	public void SemanticVersion_MetadataIsEmptyStringOrContainsInvalidCharacters_ThrowException()
	{
		Assert.Throws<ArgumentException>(() => new SemanticVersion(0, 0, 0, prerelease: null, string.Empty));

		var invalidCharacters = GetNotIdentifierCharactersString();
		foreach (var c in invalidCharacters)
		{
			var metadata = c.ToString();
			Assert.Throws<ArgumentException>(() => new SemanticVersion(0, 0, 0, prerelease: null, metadata));
		}
	}

	[Fact]
	public void SemanticVersion_MetadataShouldAcceptLettersDigitsAndDashSeparatedByDot()
	{
		var metadata = GetIdentifierCharactersWithoutDot() + "." + GetIdentifierCharactersWithoutDot();

		var semver = new SemanticVersion(0, 0, 0, prerelease: null, metadata);

		Assert.Equal(metadata, semver.Metadata);
	}

	[Fact]
	public void SemanticVersion_MetadataShouldAcceptMetadataIdentifiers()
	{
		var metadataIdentifiers = new List<string>
		{
			GetIdentifierCharactersWithoutDot(),
			GetIdentifierCharactersWithoutDot()
		};

		var semver = new SemanticVersion(0, 0, 0, prerelease: null, metadataIdentifiers);

		var expectedMetadata = string.Join('.', metadataIdentifiers);
		Assert.Equal(expectedMetadata, semver.Metadata);
	}

	[Fact]
	public void SemanticVersion_MetadataIdentifiersIsEmpty_HasMetadataShouldBeFalse()
	{
		var semver = new SemanticVersion(0, 0, 0, prerelease: null, []);

		Assert.False(semver.HasMetadata);
	}

	[Fact]
	public void SemanticVersion_MetadataIdentifiersContainsInvalidCharacters_ThrowsException()
	{
		Assert.Throws<ArgumentException>(() => new SemanticVersion(0, 0, 0, prerelease: null, [string.Empty]));

		var invalidCharacters = GetNotIdentifierCharactersString();
		foreach (var c in invalidCharacters)
			Assert.Throws<ArgumentException>(() => new SemanticVersion(0, 0, 0, prerelease: null, [c.ToString()]));
	}

	[Fact]
	public void SemanticVersion_ShouldAcceptSemanticVersionArgument()
	{
		var semverArgument = new SemanticVersion(1, 2, 3, "alpha.5", "build.42");

		var semver = new SemanticVersion(semverArgument);

		Assert.Equal(semver.Major, semverArgument.Major);
		Assert.Equal(semver.Minor, semverArgument.Minor);
		Assert.Equal(semver.Patch, semverArgument.Patch);
		Assert.Equal(semver.HasPrerelease, semverArgument.HasPrerelease);
		Assert.Equal(semver.Prerelease, semverArgument.Prerelease);
		Assert.Equal(semver.HasMetadata, semverArgument.HasMetadata);
		Assert.Equal(semver.Metadata, semverArgument.Metadata);
	}

	#endregion " Constructors tests "

	#region " ICloneable tests "

	[Fact]
	public void ICloneableClone_ValuesShouldBeEqual()
	{
		var semver = new SemanticVersion(1, 2, 3, "alpha.5", "build.42");

		var cloned = ((ICloneable)semver).Clone();

		Assert.IsType<SemanticVersion>(cloned);
		Assert.Equal(semver.Major, ((SemanticVersion)cloned).Major);
		Assert.Equal(semver.Minor, ((SemanticVersion)cloned).Minor);
		Assert.Equal(semver.Patch, ((SemanticVersion)cloned).Patch);
		Assert.Equal(semver.HasPrerelease, ((SemanticVersion)cloned).HasPrerelease);
		Assert.Equal(semver.Prerelease, ((SemanticVersion)cloned).Prerelease);
		Assert.Equal(semver.HasMetadata, ((SemanticVersion)cloned).HasMetadata);
		Assert.Equal(semver.Metadata, ((SemanticVersion)cloned).Metadata);
	}

	[Fact]
	public void Clone_ValuesShouldBeEqual()
	{
		var semver = new SemanticVersion(1, 2, 3, "alpha.5", "build.42");

		var cloned = semver.Clone();

		Assert.Equal(semver.Major, cloned.Major);
		Assert.Equal(semver.Minor, cloned.Minor);
		Assert.Equal(semver.Patch, cloned.Patch);
		Assert.Equal(semver.HasPrerelease, cloned.HasPrerelease);
		Assert.Equal(semver.Prerelease, cloned.Prerelease);
		Assert.Equal(semver.HasMetadata, cloned.HasMetadata);
		Assert.Equal(semver.Metadata, cloned.Metadata);
	}

	#endregion " ICloneable tests "

	#region " IComparable tests "

	[Fact]
	public void CompareTo_DifferentObject_ThrowsException()
	{
		var semver = new SemanticVersion(1, 2, 3);

		Assert.Throws<ArgumentException>(() => semver.CompareTo(42));
	}

	[Fact]
	public void CompareTo_Null_ShouldReturn1()
	{
		var semver = new SemanticVersion(1, 2, 3);

		var compareResult = semver.CompareTo(null);
		Assert.Equal(1, compareResult);
	}

	[Fact]
	public void CompareTo_Itself_ShouldReturn0()
	{
		var semver = new SemanticVersion(1, 2, 3);

		Assert.Equal(0, semver.CompareTo(semver));
	}

	[Theory]
	[InlineData(1, 0, 1)]
	[InlineData(0, 1, -1)]
	[InlineData(1, 1, 0)]
	public void CompareTo_MajorVersion_ShouldReturnExpectedCompareResult(int major, int otherMajor, int compareResult)
	{
		var semver = new SemanticVersion(major, 0, 0);
		var otherSemver = new SemanticVersion(otherMajor, 0, 0);

		Assert.Equal(compareResult, semver.CompareTo(otherSemver));
	}

	[Theory]
	[InlineData(1, 0, 1)]
	[InlineData(0, 1, -1)]
	[InlineData(1, 1, 0)]
	public void CompareTo_MinorVersion_ShouldReturnExpectedCompareResult(int minor, int otherMinor, int compareResult)
	{
		var semver = new SemanticVersion(0, minor, 0);
		var otherSemver = new SemanticVersion(0, otherMinor, 0);

		Assert.Equal(compareResult, semver.CompareTo(otherSemver));
	}

	[Theory]
	[InlineData(1, 0, 1)]
	[InlineData(0, 1, -1)]
	[InlineData(1, 1, 0)]
	public void CompareTo_PatchVersion_ShouldReturnExpectedCompareResult(int patch, int otherPatch, int compareResult)
	{
		var semver = new SemanticVersion(0, 0, patch);
		var otherSemver = new SemanticVersion(0, 0, otherPatch);

		Assert.Equal(compareResult, semver.CompareTo(otherSemver));
	}

	[Theory]
	[InlineData("alpha", "123", 1)]
	[InlineData("alpha.alpha", "alpha.123", 1)]
	[InlineData("123", "alpha", -1)]
	[InlineData("123.123", "123.alpha", -1)]
	[InlineData("alpha", "alpha", 0)]
	[InlineData("123", "123", 0)]
	public void CompareTo_Prerelease_AlphaNumericIdentifiersShouldBeGreaterThanNumeric(string prerelease, string otherPrerelease, int compareResult)
	{
		var semver = new SemanticVersion(0, 0, 0, prerelease);
		var otherSemver = new SemanticVersion(0, 0, 0, otherPrerelease);

		Assert.Equal(compareResult, semver.CompareTo(otherSemver));
	}

	[Theory]
	[InlineData("alpha.beta.gamma", "alpha.beta", 1)]
	[InlineData("alpha.beta", "alpha.beta.gamma", -1)]
	[InlineData("123.456.789", "123.456", 1)]
	[InlineData("123.456", "123.456.789", -1)]
	public void CompareTo_Prerelease_PrereleaseWithMoreIdentifiersCountShouldBeGreater(string prerelease, string otherPrerelease, int compareResult)
	{
		var semver = new SemanticVersion(0, 0, 0, prerelease);
		var otherSemver = new SemanticVersion(0, 0, 0, otherPrerelease);

		Assert.Equal(compareResult, semver.CompareTo(otherSemver));
	}

	[Fact]
	public void CompareTo_DifferentMetadataShouldNotAffectToCompare()
	{
		var semver = new SemanticVersion(0, 0, 0, "alpha", "metadata.1");
		var otherSemver = new SemanticVersion(0, 0, 0, "alpha", "metadata.2");

		Assert.Equal(0, semver.CompareTo(otherSemver));
	}

	#endregion " IComparable tests "

	#region " IEquatable tests "

	[Fact]
	public void Equals_Null_ShouldReturnFalse()
	{
		var semver = new SemanticVersion(1, 2, 3);

		Assert.False(semver.Equals(null));
	}

	[Fact]
	public void Equals_DifferentObject_ShouldReturnFalse()
	{
		var semver = new SemanticVersion(1, 2, 3);

		Assert.False(semver.Equals(42));
	}

	[Fact]
	public void Equals_ToItself_ShouldReturnTrue()
	{
		var semver = new SemanticVersion(1, 2, 3);

		Assert.True(semver.Equals(semver));
	}

	#endregion " IEquatable tests "

	#region " IFormattable and ToString methods tests "

	[Theory]
	[InlineData("1.2.3", "1.2.3")]
	[InlineData("1.2.3-alpha.5", "1.2.3-alpha.5")]
	[InlineData("1.2.3-alpha.5+build.42", "1.2.3-alpha.5")]
	public void ToString_ShouldReturnNormalizedString(string versionString, string expectedResult)
	{
		var semver = SemanticVersion.Parse(versionString);

		Assert.Equal(expectedResult, semver.ToString());
	}

	[Theory]
	[InlineData("1.2.3", "1.2.3")]
	[InlineData("1.2.3-alpha.5", "1.2.3")]
	[InlineData("1.2.3-alpha.5+build.42", "1.2.3")]
	public void ToVersionString_ShouldReturnVersionString(string versionString, string expectedResult)
	{
		var semver = SemanticVersion.Parse(versionString);

		Assert.Equal(expectedResult, semver.ToVersionString());
	}

	[Theory]
	[InlineData("1.2.3", "1.2.3")]
	[InlineData("1.2.3-alpha.5", "1.2.3-alpha.5")]
	[InlineData("1.2.3-alpha.5+build.42", "1.2.3-alpha.5")]
	public void ToNormalizedString_ShouldReturnNormalizedString(string versionString, string expectedResult)
	{
		var semver = SemanticVersion.Parse(versionString);

		Assert.Equal(expectedResult, semver.ToNormalizedString());
	}

	[Theory]
	[InlineData("1.2.3", "1.2.3")]
	[InlineData("1.2.3-alpha.5", "1.2.3-alpha.5")]
	[InlineData("1.2.3-alpha.5+build.42", "1.2.3-alpha.5+build.42")]
	public void ToFullString_ShouldReturnFullString(string versionString, string expectedResult)
	{
		var semver = SemanticVersion.Parse(versionString);

		Assert.Equal(expectedResult, semver.ToFullString());
	}

	[Theory]
	[InlineData("1.2.3-alpha.5+build.42", "V", "1.2.3")]
	[InlineData("1.2.3-alpha.5+build.42", "N", "1.2.3-alpha.5")]
	[InlineData("1.2.3-alpha.5+build.42", "F", "1.2.3-alpha.5+build.42")]
	[InlineData("1.2.3-alpha.5+build.42", "R", "alpha.5")]
	[InlineData("1.2.3-alpha.5+build.42", "M", "build.42")]
	[InlineData("1.2.3-alpha.5+build.42", "x", "1")]
	[InlineData("1.2.3-alpha.5+build.42", "y", "2")]
	[InlineData("1.2.3-alpha.5+build.42", "z", "3")]
	[InlineData("1.2.3-alpha.5+build.42", "xyz", "123")]
	[InlineData("1.2.3-alpha.5+build.42", "x-y-z", "1-2-3")]
	[InlineData("1.2.3-alpha.5+build.42", "x.y.z-R+M", "1.2.3-alpha.5+build.42")]
	[InlineData("1.2.3-alpha.5+build.42", "N (Test Build)", "1.2.3-alpha.5 (Test Build)")]
	public void ToString_UsingCustomFormat_ShouldReturnFormattedString(string versionString, string format, string expectedResult)
	{
		var semver = SemanticVersion.Parse(versionString);

		var formattedString = semver.ToString(format, null);

		Assert.Equal(expectedResult, formattedString);
	}

	#endregion " IFormattable and ToString methods tests "

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
	[InlineData(".")]
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
		Assert.True(left! == right!);
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

		Assert.True(leftSemver! != rightSemver!);
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

		Assert.True(leftSemver! < rightSemver);
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

		Assert.True(leftSemver > rightSemver!);
	}

	#endregion " Arithmetic operators tests "

	private static string GetIdentifierCharactersWithoutDot()
	{
		var dash = "-";
		var digits = "0123456789";
		var letters = "abcdefghijklmnopqrstuvwxyz";
		var lettersUpper = letters.ToUpper();
		return string.Concat(dash, digits, letters, lettersUpper);
	}

	private static string GetNotIdentifierCharactersString()
	{
		var allowed = string.Concat(".", GetIdentifierCharactersWithoutDot());

		var allPrintable = Enumerable.Range(32, 95) // ASCII 32 (space) to 126 (~)
			.Select(i => (char)i)
			.Where(c => !allowed.Contains(c))
			.ToArray();

		return new string(allPrintable);
	}
}
