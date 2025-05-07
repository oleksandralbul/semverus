using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Semverus.Tool.Versioning;

public class SemanticVersion : ICloneable, IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>, IFormattable, IParsable<SemanticVersion>
{
	private const char DOT = '.';
	private const char DASH = '-';
	private const char PLUS = '+';

	private const string NORMALIZED_FORMAT = "N";
	private const string FULL_FORMAT = "F";

	protected readonly Identifier[] prereleaseIdentifiers = [];
	protected readonly Identifier[] metadataIdentifiers = [];

	public int Major { get; }
	public int Minor { get; }
	public int Patch { get; }
	public string Prerelease => HasPrerelease ? string.Join(DOT, prereleaseIdentifiers) : string.Empty;
	public bool HasPrerelease => prereleaseIdentifiers.Length != 0;
	public string Metadata => HasMetadata ? string.Join(DOT, metadataIdentifiers) : string.Empty;
	public bool HasMetadata => metadataIdentifiers.Length != 0;

	public SemanticVersion(int major) : this(major, 0, 0) { }
	public SemanticVersion(int major, int minor) : this(major, minor, 0) { }
	public SemanticVersion(int major, int minor, int patch, string? prerelease = null, string? metadata = null)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(major);
		Major = major;
		ArgumentOutOfRangeException.ThrowIfNegative(minor);
		Minor = minor;
		ArgumentOutOfRangeException.ThrowIfNegative(patch);
		Patch = patch;

		if (!string.IsNullOrEmpty(prerelease))
			prereleaseIdentifiers = GetIdentifiers(prerelease);

		if (!string.IsNullOrEmpty(metadata))
			metadataIdentifiers = GetIdentifiers(metadata);
	}

	#region " ICloneable implementation "

	object ICloneable.Clone() => Clone();

	public SemanticVersion Clone() => new(Major, Minor, Patch, Prerelease, Metadata);

	#endregion " ICloneable implementation "

	#region " IComparable implementation "

	public int CompareTo(object? obj)
	{
		if (obj is not SemanticVersion version)
			throw new ArgumentException("Object is not SemanticVersion");

		return CompareTo(version);
	}

	public int CompareTo(SemanticVersion? other)
	{
		if (other == null) return 1;
		if (Major != other.Major) return Major.CompareTo(other.Major);
		if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
		if (Patch != other.Patch) return Patch.CompareTo(other.Patch);
		if (HasPrerelease != other.HasPrerelease) return -HasPrerelease.CompareTo(other.HasPrerelease);
		var minIdentifierCount = Math.Min(prereleaseIdentifiers.Length, other.prereleaseIdentifiers.Length);
		for (var i = 0; i < minIdentifierCount; i++)
		{
			var result = prereleaseIdentifiers[i].CompareTo(other.prereleaseIdentifiers[i]);
			if (result != 0) return result;
		}
		return prereleaseIdentifiers.Length.CompareTo(other.prereleaseIdentifiers.Length);
	}

	#endregion " IComparable implementation "

	#region " IEquatable implementation "

	public bool Equals(SemanticVersion? other)
		=> other != null && (ReferenceEquals(this, other) || CompareTo(other) == 0);

	public override bool Equals(object? obj)
		=> obj is SemanticVersion version && Equals(version);

	public override int GetHashCode()
		=> HashCode.Combine(Major, Minor, Patch, Prerelease, Metadata);

	#endregion " IEquatable implementation "

	#region " IFormattable implementation and ToString() override "

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		format ??= NORMALIZED_FORMAT;
		var formatter = TryGetFormatter(formatProvider, out var versionFormatter)
			? versionFormatter : SemanticVersionFormatter.Instance;

		return formatter.Format(format, this, formatProvider);
	}

	public override string ToString() => ToNormalizedString();

	public virtual string ToNormalizedString() => ToString(NORMALIZED_FORMAT, null);

	public virtual string ToFullString() => ToString(FULL_FORMAT, null);

	#endregion " IFormattable implementation and ToString() override "

	#region " IParsable implementation "

	public static SemanticVersion Parse(string s) => Parse(s, null);

	public static SemanticVersion Parse(string s, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(s);
		var index = ReadVersionNumber(s, 0, out var major);
		index = ReadVersionNumber(s, index, out var minor);
		index = ReadVersionNumber(s, index, out var patch, true);
		index = ReadPrerelease(s, index, out var prerelease);
		ReadMetadata(s, index, out var metadata);
		return new(major, minor, patch, prerelease, metadata);
	}

	public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out SemanticVersion? result)
		=> TryParse(s, null, out result);

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out SemanticVersion result)
	{
		result = null;
		try
		{
			result = Parse(s!, provider);
			return true;
		}
		catch
		{
			return false;
		}
	}

	#endregion " IParsable implementation "

	#region " Arithmetic operators implementation"

	public static bool operator ==(SemanticVersion? left, SemanticVersion? right)
		=> EqualityComparer<SemanticVersion>.Default.Equals(left, right);

	public static bool operator !=(SemanticVersion? left, SemanticVersion? right)
		=> !(left == right);

	public static bool operator <(SemanticVersion? left, SemanticVersion? right)
		=> left is null ? right is not null : left.CompareTo(right) < 0;

	public static bool operator <=(SemanticVersion? left, SemanticVersion? right)
		=> left is null || left.CompareTo(right) <= 0;

	public static bool operator >(SemanticVersion? left, SemanticVersion? right)
		=> left is not null && left.CompareTo(right) > 0;

	public static bool operator >=(SemanticVersion? left, SemanticVersion? right)
		=> left is null ? right is null : left.CompareTo(right) >= 0;

	#endregion " Arithmetic operators implementation"

	#region "Private methods and helpers "

	private static int ReadVersionNumber(string s, int index, out int number, bool isLastNumber = false)
	{
		if (index == s.Length) ThrowFormatException(s);
		var builder = new StringBuilder();
		while (index < s.Length)
		{
			var c = s[index];
			if (!char.IsDigit(c))
			{
				if (c == DASH || c == PLUS)
				{
					if (isLastNumber) break;
					ThrowFormatException(s);
				}
				if (c == DOT)
				{
					index++;
					break;
				}
			}
			builder.Append(c);
			index++;
		}
		number = int.Parse(builder.ToString());
		return index;
	}

	private static int ReadPrerelease(string s, int index, out string? prerelease)
	{
		if (index == s.Length || s[index] == PLUS)
		{
			prerelease = null;
			return index;
		}
		index++;
		var builder = new StringBuilder();
		while (index < s.Length)
		{
			var c = s[index];

			if (IsNotSemVerCharacter(c))
			{
				if (c == PLUS)
				{
					break;
				}
				ThrowFormatException(s);
			}
			builder.Append(c);
			index++;
		}

		prerelease = builder.ToString();
		if (prerelease.Length == 0) ThrowFormatException(s);
		return index;
	}

	private static void ReadMetadata(string s, int index, out string? metadata)
	{
		if (index == s.Length)
		{
			metadata = null;
			return;
		}
		if (s[index] != PLUS) ThrowFormatException(s);
		index++;
		var builder = new StringBuilder();
		while (index < s.Length)
		{
			var c = s[index];

			if (IsNotSemVerCharacter(c)) ThrowFormatException(s);
			builder.Append(c);
			index++;
		}

		metadata = builder.ToString();
		if (metadata.Length == 0) ThrowFormatException(s);
	}

	private static Identifier[] GetIdentifiers(string value)
		=> [.. value.Split(DOT).Select(p => new Identifier(p))];

	private static bool IsNotSemVerCharacter(char c)
		=> !(char.IsLetterOrDigit(c) || c == DASH || c == DOT);

	private static void ThrowFormatException(string input)
		=> throw new FormatException($"Input string {input} was not in correct format");

	private bool TryGetFormatter(IFormatProvider? formatProvider, [NotNullWhen(true)] out ICustomFormatter? formatter)
	{
		formatter = (formatProvider == null || formatProvider.GetFormat(GetType()) is not ICustomFormatter cf) ? null : cf;
		return formatter != null;
	}

	#endregion "Private methods and helpers "
}
