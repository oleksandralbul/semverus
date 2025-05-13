using Microsoft.Extensions.DependencyInjection;
using Semverus.Tool.Options;
using Semverus.Tool.Services;
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

		using var provider = BuildServiceProvider();

		var rootCommand = new RootCommand($"Semverus v{version}")
		{
			provider.GetRequiredService<ShowConfigOption>()
		};

		return await rootCommand.Parse(args).InvokeAsync();
	}

	private static ServiceProvider BuildServiceProvider()
		=> ConfigureServices().BuildServiceProvider();

	private static IServiceCollection ConfigureServices()
		=> new ServiceCollection()
			.AddTransient<ShowConfigOption>()
			.AddTransient<ShowConfigOptionAction>()
			.AddSingleton<ISemverusConfigurationService, SemverusConfigurationService>()
			;
}
