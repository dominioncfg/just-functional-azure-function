namespace JustFunctionalEvaluator.Features.Math.Requests;

/// <summary>
/// Request for validating a mathematical expression
/// </summary>
public record ValidationApiRequest
{
    /// <summary>
    /// The expression to validate.
    /// </summary>
    public string? Expression { get; init; } = string.Empty;
}