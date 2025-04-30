using Semverus.Tool.Versioning;

namespace Semverus.Tool.Tests.Versioning;

public class SemanticVersionTests
{
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
}

internal static class SemanticVersionTestExtension
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
