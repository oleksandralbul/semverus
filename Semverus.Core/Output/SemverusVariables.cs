namespace Semverus.Core.Output;

public record SemverusVariables(
	int Major,
	int Minor,
	int Patch
	)
{
	public static readonly SemverusVariables Default = new(0, 0, 0);
}
