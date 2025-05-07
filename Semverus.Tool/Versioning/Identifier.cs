
namespace Semverus.Tool.Versioning;

public readonly struct Identifier : IComparable, IComparable<Identifier>
{
	private const char DASH = '-';

	public string Value { get; }

	public bool IsNumericIdentifier => !Value.Any(c => !char.IsDigit(c));

	public Identifier(string value)
	{
		ArgumentException.ThrowIfNullOrEmpty(value);
		ValidateValue(value);
		Value = value;
	}

	public int CompareTo(object? obj)
	{
		if (obj is not Identifier identifier)
			throw new ArgumentException("Object is not Identifier");

		return CompareTo(identifier);
	}

	public int CompareTo(Identifier other)
	{
		if (!IsNumericIdentifier && other.IsNumericIdentifier) return 1;
		if (IsNumericIdentifier && !other.IsNumericIdentifier) return -1;
		var minIdentifierCount = Math.Min(Value.Length, other.Value.Length);
		for (var i = 0; i < minIdentifierCount; i++)
		{
			var result = Value[i].CompareTo(other.Value[i]);
			if (result != 0) return Math.Sign(result);
		}

		return Value.Length.CompareTo(other.Value.Length);
	}

	public override string ToString() => Value;

	private static void ValidateValue(string value)
	{
		if (value.Any(IsNotSemVerCharacter))
			throw new ArgumentException($"Value {value} is not valid identifier.");
	}

	private static bool IsNotSemVerCharacter(char c)
		=> !(char.IsLetterOrDigit(c) || c == DASH);
}
