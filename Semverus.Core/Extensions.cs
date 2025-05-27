using Microsoft.Extensions.DependencyInjection;
using Semverus.Core.Output;

namespace Semverus.Core;

public static class Extensions
{
	public static IServiceCollection AddCoreServices(this IServiceCollection services) =>
		services
		.AddTransient<ISemverusVariablesBuilder, SemverusVariablesBuilder>()
		;
}
