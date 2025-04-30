using System.Runtime.CompilerServices;

namespace Semverus.Tool.Versioning;

public class SemanticVersion : IComparable, IComparable<SemanticVersion>
{
	private const char DOT = '.';

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
