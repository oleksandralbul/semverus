using System.Runtime.CompilerServices;

namespace Semverus.Tool.Versioning;

public class SemanticVersion
{
	private const char DOT = '.';

	protected readonly IEnumerable<string> prereleaseIdentifiers = [];
	protected readonly IEnumerable<string> metadataIdentifiers = [];

	public int Major { get; }
	public int Minor { get; }
	public int Patch { get; }
	public string Prerelease => HasPrerelease ? string.Join(DOT, prereleaseIdentifiers) : string.Empty;
	public bool HasPrerelease => prereleaseIdentifiers.Any();
	public string Metadata => HasMetadata ? string.Join(DOT, metadataIdentifiers) : string.Empty;
	public bool HasMetadata => metadataIdentifiers.Any();

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
