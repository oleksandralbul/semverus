using Semverus.Core.Configuration;

namespace Semverus.Tool.Services;

internal interface ISemverusConfigurationService
{
	SemverusConfiguration GetConfiguration(string? configurationPath = null);
}
