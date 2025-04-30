using System.Reflection;

namespace Semverus.Tool;

internal class Program
{
	static void Main(string[] args)
	{
		var version = Assembly
			.GetEntryAssembly()?
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
			.InformationalVersion;

		Console.WriteLine($"Semverus v{version}");
	}
}
