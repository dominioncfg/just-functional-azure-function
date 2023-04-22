using JustFunctionalEvaluator;
using JustFunctionalEvaluator.Features.Math;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace JustFunctionalEvaluatortests.IntegrationTests;

public class FunctionsTestStartup : Startup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        base.Configure(builder);

        builder.Services.AddTransient<JustFunctionalEvaluatorFunction>();
        builder.Services.AddTransient<JustFunctionalValidationFunction>();
    }
}
