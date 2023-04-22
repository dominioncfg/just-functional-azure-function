using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace JustFunctionalEvaluatortests.IntegrationTests;

public class FunctionServerFixture
{
    public FunctionServerFixture()
    {
        var host = new HostBuilder()
            .ConfigureWebJobs(builder => builder.UseWebJobsStartup(typeof(FunctionsTestStartup), new WebJobsBuilderContext(), NullLoggerFactory.Instance))
            .Build();

        ServiceProvider = host.Services;
    }

    public IServiceProvider ServiceProvider { get; }
}