using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Semverus.Core.Versioning;

public class SemanticVersionComparer : IComparer, IComparer<SemanticVersion>, IEqualityComparer, IEqualityComparer<SemanticVersion>
{
	internal static readonly SemanticVersionComparer Instance = new();

	public int Compare(object? x, object? y)
	{
		if (x is not SemanticVersion v1)
			throw new ArgumentException($"Object {x} is not SemanticVersion");
		if (y is not SemanticVersion v2)
			throw new ArgumentException($"Object {x} is not SemanticVersion");
		return Compare(v1, v2);
	}

	public int Compare(SemanticVersion? x, SemanticVersion? y)
	{
		if (x is null) return y is null ? 0 : -1;
		if (y is null) return 1;
		if (ReferenceEquals(x, y)) return 0;
		int result = x.Major.CompareTo(y.Major);
		if (result != 0) return result;
		result = x.Minor.CompareTo(y.Minor);
		if (result != 0) return result;
		result = x.Patch.CompareTo(y.Patch);
		if (result != 0) return result;
		result = x.HasPrerelease.CompareTo(y.HasPrerelease);
		if (result != 0) return -result;
		var minCount = Math.Min(x.prereleaseIdentifiers.Length, y.prereleaseIdentifiers.Length);
		for (var i = 0; i < minCount; i++)
		{
			var xI = x.prereleaseIdentifiers[i];
			var yI = y.prereleaseIdentifiers[i];
			result = IsNumericIdentifier(xI).CompareTo(IsNumericIdentifier(yI));
			if (result != 0) return -result;
			result = xI.CompareTo(yI);
			if (result != 0) return Math.Sign(result);
		}

		return x.prereleaseIdentifiers.Length.CompareTo(y.prereleaseIdentifiers.Length);
	}

	public new bool Equals(object? x, object? y)
		=> Equals(x as SemanticVersion, y as SemanticVersion);

	public bool Equals(SemanticVersion? x, SemanticVersion? y)
		=> Compare(x, y) == 0;

	public int GetHashCode(object obj)
		=> obj is SemanticVersion version ? GetHashCode(version) : obj.GetHashCode();

	public int GetHashCode([DisallowNull] SemanticVersion obj)
	{
		var combiner = new HashCode();
		combiner.Add(obj.Major);
		combiner.Add(obj.Minor);
		combiner.Add(obj.Patch);
		foreach (var identifier in obj.prereleaseIdentifiers)
			combiner.Add(identifier);
		return combiner.ToHashCode();
	}

	private static bool IsNumericIdentifier(string identifier)
		=> !identifier.Any(c => !char.IsDigit(c));
}
