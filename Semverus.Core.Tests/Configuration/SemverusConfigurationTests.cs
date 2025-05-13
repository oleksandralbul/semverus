using Semverus.Core.Configuration;

namespace Semverus.Core.Tests.Configuration;

public class SemverusConfigurationTests
{
	[Fact]
	public void SemverusConfiguration_AllFieldsAreNull()
	{
		var config = new SemverusConfiguration();

		foreach (var property in typeof(SemverusConfiguration).GetProperties())
			Assert.Null(property.GetValue(config));
	}

	[Fact]
	public void SemverusConfiguration_SetPropertyDuringInitialization()
	{
		var expectedPropertyValue = "myValue";
		var config = new SemverusConfiguration()
		{
			AssemblyInformationalVersionFormat = expectedPropertyValue
		};

		Assert.Equal(expectedPropertyValue, config.AssemblyInformationalVersionFormat);
	}

	[Fact]
	public void Merge_OtherIsNull_NoChanges()
	{
		var expectedPropertyValue = "myValue";
		var config = new SemverusConfiguration()
		{
			AssemblyInformationalVersionFormat = expectedPropertyValue
		};

		config.Merge(null!);

		Assert.Equal(expectedPropertyValue, config.AssemblyInformationalVersionFormat);
	}

	[Fact]
	public void Merge_OtherProperyValueNull_NoChanges()
	{
		var expectedPropertyValue = "myValue";
		var config = new SemverusConfiguration()
		{
			AssemblyInformationalVersionFormat = expectedPropertyValue
		};

		var toMerge = new SemverusConfiguration();

		config.Merge(toMerge);

		Assert.Equal(expectedPropertyValue, config.AssemblyInformationalVersionFormat);
	}

	[Fact]
	public void Merge_OtherProperyDifferent_ExpectChanges()
	{
		var initialPropertyValue = "firstValue";
		var expectedPropertyValue = "newValue";
		var config = new SemverusConfiguration()
		{
			AssemblyInformationalVersionFormat = initialPropertyValue
		};

		var toMerge = new SemverusConfiguration()
		{
			AssemblyInformationalVersionFormat = expectedPropertyValue
		};

		config.Merge(toMerge);

		Assert.Equal(expectedPropertyValue, config.AssemblyInformationalVersionFormat);
	}

	[Fact]
	public void ToString_OutputCorrect()
	{
		var propertyValue = "myValue";
		var expectedString = "AssemblyInformationalVersionFormat: myValue" + Environment.NewLine;

		var config = new SemverusConfiguration()
		{
			AssemblyInformationalVersionFormat = propertyValue
		};

		var toString = config.ToString();
		Assert.Equal(expectedString, toString);
	}
}
