using System.Text;

namespace Semverus.Tool.Versioning;

internal class SemanticVersionFormatter : ICustomFormatter
{
	private const char NORMALIZED = 'N';
	private const char FULL = 'F';
	private const char VERSION = 'V';
	private const char PRERELEASE = 'R';
	private const char METADATA = 'M';
	private const char MAJOR = 'x';
	private const char MINOR = 'y';
	private const char PATCH = 'z';
	private const char DOT = '.';
	private const char DASH = '-';
	private const char PLUS = '+';

	public static ICustomFormatter Instance => new SemanticVersionFormatter();

	public string Format(string? format, object? arg, IFormatProvider? formatProvider)
	{
		ArgumentNullException.ThrowIfNull(arg);
		if (arg is string s) return s;
		if (arg is not SemanticVersion v)
			throw new ArgumentException($"Type {arg.GetType()} is not supported.");
		format ??= NORMALIZED.ToString();

		return Format(format, v);
	}

	private static string Format(string format, SemanticVersion version)
	{
		var builder = new StringBuilder();

		foreach (var c in format)
		{
			switch (c)
			{
				case NORMALIZED: AppendNormalized(builder, version); break;
				case FULL: AppendFull(builder, version); break;
				case VERSION: AppendVersion(builder, version); break;
				case PRERELEASE: builder.Append(version.Prerelease); break;
				case METADATA: builder.Append(version.Metadata); break;
				case MAJOR: builder.Append(version.Major); break;
				case MINOR: builder.Append(version.Minor); break;
				case PATCH: builder.Append(version.Patch); break;
				default: builder.Append(c); break;
			}
		}

		return builder.ToString();
	}

	private static void AppendVersion(StringBuilder builder, SemanticVersion version)
	{
		builder.Append(version.Major);
		builder.Append(DOT);
		builder.Append(version.Minor);
		builder.Append(DOT);
		builder.Append(version.Patch);
	}

	private static void AppendFull(StringBuilder builder, SemanticVersion version)
	{
		AppendNormalized(builder, version);
		if (!version.HasMetadata) return;
		builder.Append(PLUS);
		builder.Append(version.Metadata);
	}

	private static void AppendNormalized(StringBuilder builder, SemanticVersion version)
	{
		AppendVersion(builder, version);
		if (!version.HasPrerelease) return;
		builder.Append(DASH);
		builder.Append(version.Prerelease);
	}
}
