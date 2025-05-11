using System.CommandLine;
using System.Reflection;

namespace Semverus.Tool;

internal class Program
{
	static async Task<int> Main(string[] args)
	{
		var version = Assembly
			.GetEntryAssembly()?
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
			.InformationalVersion;

		var rootCommand = new RootCommand($"Semverus v{version}");

		return await rootCommand.InvokeAsync(args);
	}
}
