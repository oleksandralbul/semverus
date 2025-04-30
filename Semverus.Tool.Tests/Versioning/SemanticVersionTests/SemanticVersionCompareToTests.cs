using Semverus.Tool.Versioning;

namespace Semverus.Tool.Tests.Versioning.SemanticVersionTests;

public class SemanticVersionCompareToTests
{
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
}
