using FluentAssertions;
using JustFunctionalEvaluator.Features.Math;
using JustFunctionalEvaluator.Features.Math.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace JustFunctionalEvaluatortests.IntegrationTests;

[Collection(FunctionServerFixtureTestsCollection.Name)]
public class WhenValidatingExpressions
{
    private readonly JustFunctionalValidationFunction _function;
    private readonly FunctionServerFixture _fixture;

    public WhenValidatingExpressions(FunctionServerFixture testsInitializer)
    {
        _fixture = testsInitializer;
        _function = _fixture.ServiceProvider.GetRequiredService<JustFunctionalValidationFunction>();
    }


    [Fact]
    public void ValidationReturnsTrueWhenExpressionIsValidConstantExpression()
    {
        const string fx = "3+2";

        var apiResult = GetAndValidateExpression(fx);

        apiResult.Should().NotBeNull();
        apiResult.Success.Should().BeTrue();
        apiResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidationReturnsTrueWhenExpressionIsValidSingleVariable()
    {
        const string fx = "X+2";
        string[] variables = new[] { "X" };

        var apiResult = GetAndValidateExpression(fx, variables);

        apiResult.Should().NotBeNull();
        apiResult.Success.Should().BeTrue();
        apiResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidationReturnsTrueWhenExpressionIsValidMultipleVariables()
    {
        const string fx = "X+Y";
        string[] variables = new[] { "X", "Y" };

        var apiResult = GetAndValidateExpression(fx, variables);

        apiResult.Should().NotBeNull();
        apiResult.Success.Should().BeTrue();
        apiResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidationReturnsFalseWhenExpressionIsEmpty()
    {
        const string fx = "";

        var apiResult = GetAndValidateExpression(fx);

        apiResult.Should().NotBeNull();
        apiResult.Success.Should().BeFalse();
        apiResult.Errors.Should().NotBeEmpty();
        foreach (var error in apiResult.Errors)
            error.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidationReturnsFalseWhenExpressionIsInvalidSyntaxError()
    {
        const string fx = "(3+2";

        var apiResult = GetAndValidateExpression(fx);

        apiResult.Should().NotBeNull();
        apiResult.Success.Should().BeFalse();
        apiResult.Errors.Should().NotBeEmpty();
        foreach (var error in apiResult.Errors)
            error.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidationReturnsFalseWhenExpressionIsInvalidMissingVariable()
    {
        const string fx = "X+2";

        var apiResult = GetAndValidateExpression(fx);

        apiResult.Should().NotBeNull();
        apiResult.Success.Should().BeFalse();
        apiResult.Errors.Should().NotBeEmpty();
        foreach (var error in apiResult.Errors)
            error.Should().NotBeEmpty();
    }

    [Fact]
    public void ValidationReturnsFalseWhenExpressionIsInvalidWrongVariable()
    {
        const string fx = "X+2";
        string[] variables = new[] { "Y" };
        var apiResult = GetAndValidateExpression(fx, variables);

        apiResult.Should().NotBeNull();
        apiResult.Success.Should().BeFalse();
        apiResult.Errors.Should().NotBeEmpty();
        foreach (var error in apiResult.Errors)
            error.Should().NotBeEmpty();
    }


    private ValidationApiResponse GetAndValidateExpression(string fx, IEnumerable<string>? variables = null)
    {
        var apiRequest = new ValidationApiRequest()
        {
            Expression = fx
        };

        var httpRequest = new DefaultHttpContext();
        AddVariablesToQueryString(httpRequest, variables ?? new List<string>());
        var result = _function.Run(apiRequest, httpRequest.Request);

        result.Should().NotBeNull();

        var apiResponse = result as OkObjectResult;
        apiResponse.Should().NotBeNull();
        apiResponse!.StatusCode.Should().Be((int)HttpStatusCode.OK);

        apiResponse!.Value.Should().BeOfType<ValidationApiResponse>();

        return (ValidationApiResponse)apiResponse.Value;
    }

    private static void AddVariablesToQueryString(DefaultHttpContext httpRequest, IEnumerable<string> variables)
    {
        var queryparams = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
        var stringValues = new Microsoft.Extensions.Primitives.StringValues(variables.ToArray());
        queryparams.Add("Variables", stringValues);
        httpRequest.Request.Query = new QueryCollection(queryparams);
    }
}

