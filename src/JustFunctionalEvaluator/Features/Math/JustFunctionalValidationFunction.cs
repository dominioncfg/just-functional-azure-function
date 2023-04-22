using JustFunctional.Core;
using JustFunctionalEvaluator.Features.Math.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using JustFunctionalEvaluator.Extensions;

namespace JustFunctionalEvaluator.Features.Math;

public class JustFunctionalValidationFunction
{
    private readonly IFunctionFactory _functionFactory;

    public JustFunctionalValidationFunction(IFunctionFactory functionFactory)
    {
        _functionFactory = functionFactory;
    }

    [FunctionName("JustFunctionalValidationFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/math/validate")][FromQuery] ValidationApiRequest request, HttpRequest req)
    {
        var variables = req.Query.ParseArrayFromQueryString("Variables");
        var allowedVariables = new PredefinedVariablesProvider(variables ?? Array.Empty<string>());
        var result = _functionFactory.TryCreate(request.Expression ?? string.Empty, allowedVariables);

        var response = new ValidationApiResponse()
        {
            Success = result.Success,
            Errors = result.Errors,
        };
        return new OkObjectResult(response);
    }
}
