using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JustFunctionalEvaluator.Extensions;

public static class HttpExtensions
{
    public static Dictionary<string, decimal> ParseDictionaryFromQueryString(this IQueryCollection query, string paramName)
    {
        var dictStart = $"{paramName}[";
        const string dictEnd = "]";

        if (query is null || !query.Any())
            return new Dictionary<string, decimal>();

        var variablesQueryParams = query
            .Where(x =>
                        x.Key.StartsWith(dictStart, StringComparison.InvariantCultureIgnoreCase) &&
                        x.Key.EndsWith(dictEnd)
                  )
            .ToArray();

        var result = new Dictionary<string, decimal>();
        foreach (var param in variablesQueryParams)
        {
            var variableName = param.Key
                .Replace(dictStart, string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace(dictEnd, string.Empty);

            var hasParsed = decimal.TryParse(param.Value, out var val);

            if (hasParsed)
                result.Add(variableName, val);
        }
        return result;
    }

    public static string[] ParseArrayFromQueryString(this IQueryCollection query, string paramName)
    {
        if (query is null || !query.Any())
            return Array.Empty<string>();

        var variablesQueryParams = query
            .Where(x =>
                      x.Key.Equals(paramName, StringComparison.InvariantCultureIgnoreCase)
                  )
            .FirstOrDefault();

        var result = variablesQueryParams.Value.ToArray() ?? Array.Empty<string>().ToArray();
        return result;
    }
}
