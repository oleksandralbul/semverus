namespace Semverus.Core.Configuration;

public class SemverusConfiguration
{
	public string? AssemblyInformationalVersionFormat { get; set; }

	public void Merge(SemverusConfiguration other)
	{
		if (other == null) return;
		if (other.AssemblyInformationalVersionFormat != null)
			AssemblyInformationalVersionFormat = other.AssemblyInformationalVersionFormat;
	}

	public override string ToString()
	{
		var result = string.Empty;
		foreach (var property in GetType().GetProperties())
		{
			var propertyName = property.Name;
			var propertyValue = property.GetValue(this);
			result += $"{propertyName}: {propertyValue?.ToString()}" + Environment.NewLine;
		}
		return result;
	}
}
