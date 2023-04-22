using JustFunctional.Core;
using JustFunctionalEvaluator.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net;

namespace JustFunctionalEvaluator.Features.Math;

public class JustFunctionalEvaluatorFunction
{
    private readonly IFunctionFactory _functionFactory;

    public JustFunctionalEvaluatorFunction(IFunctionFactory functionFactory)
    {
        _functionFactory = functionFactory;
    }

    [FunctionName("JustFunctionalEvaluatorFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v2/math/evaluate")]
                             [FromQuery] EvaluationApiRequest apiRequest,
                             HttpRequest req)
    {
        try
        {
            var request = apiRequest with
            {
                //Azure Functions's model binder is not capable of doing this:
                Variables = req.Query.ParseDictionaryFromQueryString(nameof(apiRequest.Variables)),
            };

            var fx = _functionFactory.Create(request.Expression);
            var result = fx.Evaluate(new EvaluationContext(request.Variables ?? new Dictionary<string, decimal>()));

            return new OkObjectResult(new EvaluationApiResponse()
            {
                Result = result,
            });
        }
        catch (JustFunctionalBaseException e)
        {
            return new BadRequestObjectResult(new ProblemDetails()
            {
                Status = (int)HttpStatusCode.BadRequest,
                Detail = e.Message
            });
        }
        catch
        {
            var error = new ProblemDetails()
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "Ups! something went wrong!",
            };
            var result = new ObjectResult(error)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
            return result;
        }
    }
}

