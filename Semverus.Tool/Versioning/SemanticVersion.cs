using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace Semverus.Tool.Versioning;

public class SemanticVersion : ICloneable, IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>, IParsable<SemanticVersion>
{
	private const char DOT = '.';
	private const char DASH = '-';
	private const char PLUS = '+';

	protected readonly string[] prereleaseIdentifiers = [];
	protected readonly string[] metadataIdentifiers = [];

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
		{
			prereleaseIdentifiers = prerelease.Split(DOT);
			foreach (string identifier in prereleaseIdentifiers)
				IdentifierNotValidException.ThrowIfNotValid(identifier, prerelease);
		}

		if (!string.IsNullOrEmpty(metadata))
		{
			metadataIdentifiers = metadata.Split(DOT);
			foreach (string identifier in metadataIdentifiers)
				IdentifierNotValidException.ThrowIfNotValid(identifier, metadata);
		}
	}

	public override string ToString()
		=> $"{Major}.{Minor}.{Patch}";

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

	private static bool IsNotSemVerCharacter(char c)
		=> !(char.IsLetterOrDigit(c) || c == DASH || c == DOT);

	private static void ThrowFormatException(string input)
		=> throw new FormatException($"Input string {input} was not in correct format");

	#endregion "Private methods and helpers "
}

public class IdentifierNotValidException : Exception
{
	private IdentifierNotValidException(string identifier, string identifierString, string label)
		: base($"An identifier \"{identifier}\" in a {label} label ({identifierString}) is not a valid SemVer 2.0 identifier.") { }

	public static void ThrowIfNotValid(string identifier, string identifiersString, [CallerArgumentExpression(nameof(identifiersString))] string? label = null)
	{
		if (identifier.Length == 0 || identifier.Any(IsNotSemVerCharacter))
			throw new IdentifierNotValidException(identifier, identifiersString, label!);
	}

	private static bool IsNotSemVerCharacter(char c)
		=> !(char.IsLetterOrDigit(c) || c == '-');
}
