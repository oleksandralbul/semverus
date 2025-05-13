using Semverus.Core.Configuration;
using YamlDotNet.Serialization;

namespace Semverus.Tool.Services;

internal class SemverusConfigurationService : ISemverusConfigurationService
{
	public SemverusConfiguration GetConfiguration(string? configurationPath = null)
	{
		configurationPath ??= GetConfigurationPath();
		var configuration = GetDefaultConfiguration();
		if (configurationPath != null)
		{
			var userConfiguration = ReadUserConfiguration(configurationPath);
			configuration.Merge(userConfiguration);
		}
		return configuration;
	}

	private static SemverusConfiguration ReadUserConfiguration(string configurationPath)
	{
		using var reader = new StreamReader(configurationPath);
		var deserializer = new DeserializerBuilder()
			.WithNamingConvention(KebabCaseNamingConvention.Instance)
			.Build();

		var config = deserializer.Deserialize<SemverusConfiguration>(reader);

		return config;
	}

	private static string? GetConfigurationPath()
	{
		var configurationFileNames = new string[] { ".semverus.yml", ".semverus.yaml" };
		foreach (var fileName in configurationFileNames)
		{
			var configurationFilePath = Path.Combine(AppContext.BaseDirectory, fileName);
			if (File.Exists(configurationFilePath)) return configurationFilePath;
		}
		return null;
	}

	private static SemverusConfiguration GetDefaultConfiguration() => new()
	{
		AssemblyInformationalVersionFormat = "{Major}.{Minor}.{Patch}-{Prerelease}+{Metadata}"
	};

	private class KebabCaseNamingConvention : INamingConvention
	{
		public static readonly KebabCaseNamingConvention Instance = new();

		public string Apply(string value) => ToKebabCase(value);

		public string Reverse(string value) => throw new NotImplementedException();

		private static string ToKebabCase(string value) =>
			string.Concat(value.Select((ch, i) =>
				i > 0 && char.IsUpper(ch) ? "-" + char.ToLower(ch) : char.ToLower(ch).ToString()));
	}
}
