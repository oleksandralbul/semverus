using System.Text;

namespace Semverus.Core.Versioning;

public class SemanticVersionFormatter : ICustomFormatter, IFormatProvider
{
	internal static readonly SemanticVersionFormatter Instanse = new();

	public object? GetFormat(Type? formatType)
	{
		ArgumentNullException.ThrowIfNull(formatType);
		return formatType == typeof(SemanticVersion) ? this : null;
	}

	public string Format(string? format, object? arg, IFormatProvider? formatProvider)
	{
		if (arg is null) return string.Empty;
		if (arg is string str) return str;
		if (arg is not SemanticVersion version)
			throw new ArgumentException($"Type {arg.GetType()} is not supported.");
		if (string.IsNullOrEmpty(format)) format = Constants.NORMALIZED_FORMAT;
		return Format(format, version);
	}

	private static string Format(string format, SemanticVersion version)
	{
		var builder = new StringBuilder();
		foreach (var c in format)
		{
			switch (c)
			{
				case Constants.MAJOR:
					builder.Append(version.Major); break;
				case Constants.MINOR:
					builder.Append(version.Minor); break;
				case Constants.PATCH:
					builder.Append(version.Patch); break;
				case Constants.PRERELEASE:
					builder.Append(version.Prerelease); break;
				case Constants.METADATA:
					builder.Append(version.Metadata); break;
				case Constants.VERSION:
					AppendVersion(builder, version); break;
				case Constants.NORMALIZED:
					AppendNormalized(builder, version); break;
				case Constants.FULL:
					AppendFull(builder, version); break;
				default:
					builder.Append(c); break;
			}
		}
		return builder.ToString();
	}

	private static void AppendVersion(StringBuilder builder, SemanticVersion version)
	{
		builder.Append(version.Major);
		builder.Append(Constants.DOT);
		builder.Append(version.Minor);
		builder.Append(Constants.DOT);
		builder.Append(version.Patch);
	}

	private static void AppendNormalized(StringBuilder builder, SemanticVersion version)
	{
		AppendVersion(builder, version);
		if (!version.HasPrerelease) return;
		builder.Append(Constants.DASH);
		builder.Append(version.Prerelease);
	}

	private static void AppendFull(StringBuilder builder, SemanticVersion version)
	{
		AppendNormalized(builder, version);
		if (!version.HasMetadata) return;
		builder.Append(Constants.PLUS);
		builder.Append(version.Metadata);
	}
}
