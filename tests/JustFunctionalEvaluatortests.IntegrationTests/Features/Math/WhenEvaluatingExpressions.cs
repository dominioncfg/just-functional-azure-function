using FluentAssertions;
using JustFunctional.Core;
using JustFunctionalEvaluator.Features.Math;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net;

namespace JustFunctionalEvaluatortests.IntegrationTests;

[Collection(FunctionServerFixtureTestsCollection.Name)]
public class WhenEvaluatingExpressions
{
    private readonly JustFunctionalEvaluatorFunction _function;
    private readonly FunctionServerFixture _fixture;

    public WhenEvaluatingExpressions(FunctionServerFixture testsInitializer)
    {
        _fixture = testsInitializer;
        _function = _fixture.ServiceProvider.GetRequiredService<JustFunctionalEvaluatorFunction>();
    }


    [Fact]
    public void EvaluationReturnsOkWhenExpressionIsValidConstantExpression()
    {
        const string fx = "3+2";

        var apiResult = GetAndEvaluateExpression(fx);

        apiResult.Should().NotBeNull();
        apiResult.Result.Should().Be(5);
    }

    [Fact]
    public void EvaluationReturnsOkWhenExpressionIsValidSingleVariable()
    {
        const string fx = "X+2";
        var variables = new Dictionary<string, decimal> { ["X"] = 3 };

        var apiResult = GetAndEvaluateExpression(fx, variables);

        apiResult.Should().NotBeNull();
        apiResult.Result.Should().Be(5);
    }

    [Fact]
    public void EvaluationReturnsOkWhenExpressionIsValidMultipleVariables()
    {
        const string fx = "X+Y";
        var variables = new Dictionary<string, decimal> { ["X"] = 3, ["Y"] = 8 };

        var apiResult = GetAndEvaluateExpression(fx, variables);

        apiResult.Should().NotBeNull();
        apiResult.Result.Should().Be(11);
    }


    [Fact]
    public void EvaluationReturnsBadRequestWhenExpressionIsEmpty()
    {
        const string fx = "";
        var problemDetails = GetAndExpectBadRequest(fx);
        problemDetails.Should().NotBeNull();
        problemDetails.Detail.Should().Be("Expression can't be empty");
    }


    [Fact]
    public void EvaluationReturnsBadRequestWhenExpressionIsInvalidSyntaxError()
    {
        const string fx = "(3+2";
        var problemDetails = GetAndExpectBadRequest(fx);
        problemDetails.Should().NotBeNull();
        problemDetails.Detail.Should().Be("There is a '(' without corresponding ')'");
    }

    [Fact]
    public void EvaluationReturnsBadRequestWhenExpressionIsInvalidMissingVariable()
    {
        const string fx = "X+2";
        var problemDetails = GetAndExpectBadRequest(fx);
        problemDetails.Should().NotBeNull();
        problemDetails.Detail.Should().Be("Syntax error after '', at position 3. Expected operator/operand.");
    }

    [Fact]
    public void EvaluationReturnsBadRequestWhenExpressionIsInvalidWrongVariable()
    {
        const string fx = "X+2";
        var variables = new Dictionary<string, decimal> { ["Y"] = 3 };
        var problemDetails = GetAndExpectBadRequest(fx,variables);
        problemDetails.Should().NotBeNull();
        problemDetails.Detail.Should().Be("Syntax error after '', at position 3. Expected operator/operand.");
    }

    private EvaluationApiResponse GetAndEvaluateExpression(string fx, Dictionary<string, decimal>? variables = null)
    {
        var apiRequest = new EvaluationApiRequest()
        {
            Expression = fx,
            Variables = variables ?? new Dictionary<string, decimal>(),
        };

        var httpRequest = new DefaultHttpContext();
        AddVariablesToQueryString(httpRequest, variables ?? new Dictionary<string, decimal>());
        var result = _function.Run(apiRequest, httpRequest.Request);

        result.Should().NotBeNull();

        var apiResponse = result as OkObjectResult;
        apiResponse.Should().NotBeNull();
        apiResponse!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        apiResponse!.Value.Should().BeOfType<EvaluationApiResponse>();

        return (EvaluationApiResponse)apiResponse.Value;
    }

    private ProblemDetails GetAndExpectBadRequest(string fx, Dictionary<string, decimal>? variables = null)
    {
        var apiRequest = new EvaluationApiRequest()
        {
            Expression = fx,
            Variables = variables ?? new Dictionary<string, decimal>(),
        };

        var httpRequest = new DefaultHttpContext();
        AddVariablesToQueryString(httpRequest, variables ?? new Dictionary<string, decimal>());
        var result = _function.Run(apiRequest, httpRequest.Request);

        result.Should().NotBeNull();

        var apiResponse = result as BadRequestObjectResult;
        apiResponse.Should().NotBeNull();
        apiResponse!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        apiResponse!.Value.Should().BeOfType<ProblemDetails>();

        return (ProblemDetails)apiResponse.Value;
    }

    private static void AddVariablesToQueryString(DefaultHttpContext httpRequest, Dictionary<string, decimal> variables)
    {
        var queryparams = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
        foreach (var variable in variables)
        {
            queryparams.Add($"Variables[{variable.Key}]", variable.Value.ToString());
        }
        httpRequest.Request.Query = new QueryCollection(queryparams);
    }
}

