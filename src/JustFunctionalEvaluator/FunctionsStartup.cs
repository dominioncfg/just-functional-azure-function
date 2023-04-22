using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(JustFunctionalEvaluator.Startup))]


namespace JustFunctionalEvaluator;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddJustFunctional();
    }
}
