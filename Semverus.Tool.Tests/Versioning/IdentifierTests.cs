using Semverus.Tool.Versioning;

namespace Semverus.Tool.Tests.Versioning;

public class IdentifierTests
{
	[Theory]
	[InlineData("-")]
	[InlineData("42")]
	[InlineData("alpha")]
	[InlineData("4-2")]
	[InlineData("alpha-beta")]
	public void Identifier_ShouldCreateIfValueCorrect(string value)
		=> Assert.Equal(value, new Identifier(value).Value);

	[Fact]
	public void Identifier_ShouldThrowArgumentNullExceptionIfValueNull()
		=> Assert.Throws<ArgumentNullException>(() => new Identifier(null!));

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(".")]
	[InlineData("alpha.beta")]
	[InlineData("12 34")]
	public void Identifier_ShouldThrowArgumentExceptionIfValueNotValid(string value)
		=> Assert.Throws<ArgumentException>(() => new Identifier(value));

	[Fact]
	public void Identifier_ValueContainOnlyDigits_IsNumericIdentifierShouldBeTrue()
		=> Assert.True(new Identifier("0123456789").IsNumericIdentifier);

	[Theory]
	[InlineData("alpha")]
	[InlineData("a1234")]
	[InlineData("-42")]
	[InlineData("-42-abc")]
	[InlineData("-")]
	public void Identifier_ValueContainOnlyDigits_IsNumericIdentifierShouldBeFalse(string value)
		=> Assert.False(new Identifier(value).IsNumericIdentifier);

	[Theory]
	[InlineData("123", "123", 0)]
	[InlineData("alpha", "alpha", 0)]
	[InlineData("-", "-", 0)]
	[InlineData("alpha1", "alpha1", 0)]
	[InlineData("alpha", "beta", -1)]
	[InlineData("beta", "alpha", 1)]
	[InlineData("123", "456", -1)]
	[InlineData("456", "123", 1)]
	[InlineData("alpha", "123", 1)]
	[InlineData("-beta", "beta", -1)]
	[InlineData("beta1", "beta2",-1)]
	public void CompareTo_ShouldCompareCorrect(string left, string right, int compareResult)
		=> Assert.Equal(new Identifier(left).CompareTo(new Identifier(right)), compareResult);
}
