using Semverus.Tool.Services;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Semverus.Tool.Options;

internal class ShowConfigOption : Option<bool>
{
	private readonly ShowConfigOptionAction action;

	public ShowConfigOption(ShowConfigOptionAction action) : base("--config", "-c")
	{
		Description = "Show configuration for this project";

		this.action = action;
	}

	public override CommandLineAction? Action => action;
}

internal class ShowConfigOptionAction(ISemverusConfigurationService semverusConfigurationService) : SynchronousCommandLineAction
{
	private readonly ISemverusConfigurationService semverusConfigurationService = semverusConfigurationService;

	public override int Invoke(ParseResult parseResult)
	{
		var config = semverusConfigurationService.GetConfiguration();
		parseResult.Configuration.Output.WriteLine(config.ToString());
		return 1;
	}
}
