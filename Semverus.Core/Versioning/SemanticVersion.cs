using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Semverus.Core.Versioning;

public class SemanticVersion : ICloneable, IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>, IFormattable, IParsable<SemanticVersion>
{
	internal protected readonly string[] prereleaseIdentifiers = [];
	internal protected readonly string[] metadataIdentifiers = [];

	public static readonly SemanticVersion Zero = new(0, 0, 0);

	public int Major { get; }
	public int Minor { get; }
	public int Patch { get; }

	public string? Prerelease => HasPrerelease ? string.Join(Constants.DOT, prereleaseIdentifiers) : null;
	public string? Metadata => HasMetadata ? string.Join(Constants.DOT, metadataIdentifiers) : null;

	public bool HasPrerelease => prereleaseIdentifiers.Length != 0;
	public bool HasMetadata => metadataIdentifiers.Length != 0;

	public SemanticVersion(SemanticVersion other) : this(other.Major, other.Minor, other.Patch, other.prereleaseIdentifiers, other.metadataIdentifiers) { }
	public SemanticVersion(int major, int minor, int patch) : this(major, minor, patch, [], []) { }
	public SemanticVersion(int major, int minor, int patch, string? prerelease) : this(major, minor, patch, ParseIdentifiers(prerelease)) { }
	public SemanticVersion(int major, int minor, int patch, IEnumerable<string> prereleaseIdentifiers) : this(major, minor, patch, prereleaseIdentifiers, []) { }
	public SemanticVersion(int major, int minor, int patch, string? prerelease, string? metadata) : this(major, minor, patch, ParseIdentifiers(prerelease), ParseIdentifiers(metadata)) { }
	public SemanticVersion(int major, int minor, int patch, string? prerelease, IEnumerable<string> metadataIdentifiers) : this(major, minor, patch, ParseIdentifiers(prerelease), metadataIdentifiers) { }
	public SemanticVersion(int major, int minor, int patch, IEnumerable<string> prereleaseIdentifiers, string? metadata) : this(major, minor, patch, prereleaseIdentifiers, ParseIdentifiers(metadata)) { }
	public SemanticVersion(int major, int minor, int patch, IEnumerable<string> prereleaseIdentifiers, IEnumerable<string> metadataIdentifiers)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(major);
		Major = major;
		ArgumentOutOfRangeException.ThrowIfNegative(minor);
		Minor = minor;
		ArgumentOutOfRangeException.ThrowIfNegative(patch);
		Patch = patch;

		prereleaseIdentifiers ??= [];
		ValidateIdentifiers(prereleaseIdentifiers);
		this.prereleaseIdentifiers = [.. prereleaseIdentifiers];

		metadataIdentifiers ??= [];
		ValidateIdentifiers(metadataIdentifiers);
		this.metadataIdentifiers = [.. metadataIdentifiers];
	}

	#region " ICloneable implementation "

	object ICloneable.Clone() => Clone();

	public SemanticVersion Clone()
		=> new(Major, Minor, Patch, prereleaseIdentifiers, metadataIdentifiers);

	#endregion " ICloneable implementation "

	#region " IComparable implementation "

	public int CompareTo(object? obj)
	{
		if (obj is not SemanticVersion version)
			throw new ArgumentException("Object is not SemanticVersion");
		return CompareTo(version);
	}

	public int CompareTo(SemanticVersion? other)
		=> SemanticVersionComparer.Instance.Compare(this, other);

	#endregion " IComparable implementation "

	#region " IEquatable, Equals and GetHashCode methods "

	public override bool Equals(object? obj)
		=> obj is SemanticVersion version && Equals(version);

	public bool Equals(SemanticVersion? other)
		=> SemanticVersionComparer.Instance.Equals(this, other);

	public override int GetHashCode()
		=> SemanticVersionComparer.Instance.GetHashCode(this);

	#endregion " IEquatable, Equals and GetHashCode methods "

	#region " IFormattable and ToString methods "

	public string ToString(string? format, IFormatProvider? formatProvider)
	{
		format ??= Constants.NORMALIZED_FORMAT;
		var formatter = TryGetFormatter(formatProvider, out var customFormatter) ? customFormatter : SemanticVersionFormatter.Instanse;
		return formatter.Format(format, this, formatProvider);
	}

	public override string ToString() => ToNormalizedString();

	public string ToVersionString() => ToFormattedString(Constants.VERSION_FORMAT);

	public string ToNormalizedString() => ToFormattedString(Constants.NORMALIZED_FORMAT);

	public string ToFullString() => ToFormattedString(Constants.FULL_FORMAT);

	private string ToFormattedString(string format) => ToString(format, SemanticVersionFormatter.Instanse);

	private static bool TryGetFormatter(IFormatProvider? formatProvider, [NotNullWhen(true)] out ICustomFormatter? formatter)
	{
		formatter = (formatProvider == null || formatProvider.GetFormat(typeof(SemanticVersion)) is not ICustomFormatter customFormatter) ? null : customFormatter;
		return formatter != null;
	}

	#endregion " IFormattable and ToString methods "

	#region " IParsable implementation "

	public static SemanticVersion Parse(string s) => Parse(s, null);

	public static SemanticVersion Parse(string s, IFormatProvider? provider)
	{
		ArgumentNullException.ThrowIfNull(s);
		var enumerator = s.GetEnumerator();
		var major = ReadVersionNumber(enumerator);
		var minor = ReadVersionNumber(enumerator);
		var patch = ReadVersionNumber(enumerator, true);
		var prerelease = ReadPrerelease(enumerator);
		var metadata = ReadMetadata(enumerator);
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

	private static bool IsEnumerationCompleted(CharEnumerator enumerator)
		=> enumerator.Clone() is CharEnumerator e && !e.MoveNext();

	private static bool IsSemverCharacter(char c)
		=> char.IsLetterOrDigit(c) || IsSemverSeparator(c);

	private static bool IsSemverSeparator(char c)
		=> c is Constants.DOT || c is Constants.DASH || c is Constants.PLUS;

	private static int ReadVersionNumber(CharEnumerator enumerator, bool isPatchNumber = false)
	{
		if (IsEnumerationCompleted(enumerator)) ThrowFormatException();
		var builder = new StringBuilder();
		while (enumerator.MoveNext())
		{
			var c = enumerator.Current;
			if (char.IsDigit(c))
			{
				builder.Append(c);
				continue;
			}
			if (IsSemverSeparator(c))
			{
				if (c is Constants.DOT)
				{
					if (isPatchNumber) ThrowFormatException();
					break;
				}
				if (isPatchNumber) break;
			}
			ThrowFormatException();
		}
		return int.Parse(builder.ToString());
	}

	private static string? ReadPrerelease(CharEnumerator enumerator)
	{
		if (IsEnumerationCompleted(enumerator) || enumerator.Current is Constants.PLUS) return null;
		var builder = new StringBuilder();
		while (enumerator.MoveNext())
		{
			var c = enumerator.Current;
			if (IsSemverCharacter(c))
			{
				if (c is Constants.PLUS) break;
				builder.Append(c);
				continue;
			}
			ThrowFormatException();
		}
		if (builder.Length == 0) ThrowFormatException();
		return builder.ToString();
	}

	private static string? ReadMetadata(CharEnumerator enumerator)
	{
		if (IsEnumerationCompleted(enumerator)) return null;
		var builder = new StringBuilder();
		while (enumerator.MoveNext())
		{
			var c = enumerator.Current;
			if (IsSemverCharacter(c))
			{
				if (c is Constants.PLUS) ThrowFormatException();
				builder.Append(c);
				continue;
			}
			ThrowFormatException();
		}
		if (builder.Length == 0) ThrowFormatException();
		return builder.ToString();
	}

	private static void ThrowFormatException()
		=> throw new FormatException("Input string is not in the correct format.");

	#endregion " IParsable implementation "

	#region " Arithmetic operators "

	public static bool operator ==(SemanticVersion left, SemanticVersion right)
		=> SemanticVersionComparer.Instance.Equals(left, right);

	public static bool operator !=(SemanticVersion left, SemanticVersion right)
		=> !(left == right);

	public static bool operator <(SemanticVersion left, SemanticVersion right)
		=> SemanticVersionComparer.Instance.Compare(left, right) < 0;

	public static bool operator <=(SemanticVersion left, SemanticVersion right)
		=> SemanticVersionComparer.Instance.Compare(left, right) <= 0;

	public static bool operator >(SemanticVersion left, SemanticVersion right)
		=> !(left <= right);

	public static bool operator >=(SemanticVersion left, SemanticVersion right)
		=> !(left < right);

	#endregion " Arithmetic operators "

	#region " Private methods and helpers "

	private static string[] ParseIdentifiers(string? identifiersString)
		=> identifiersString?.Split(Constants.DOT) ?? [];

	private static void ValidateIdentifiers(IEnumerable<string> identifiers)
	{
		foreach (var identifier in identifiers)
			ThrowIfIdentifierNotValid(identifier);
	}

	private static void ThrowIfIdentifierNotValid(string? identifier)
	{
		if (string.IsNullOrWhiteSpace(identifier) || identifier.Any(c => !IsIdentifierCharacter(c)))
			throw new ArgumentException($"Identifier \"{identifier}\" is not valid");
	}

	private static bool IsIdentifierCharacter(char c)
		=> char.IsLetterOrDigit(c) || c is Constants.DASH;

	#endregion " Private methods and helpers "
}
