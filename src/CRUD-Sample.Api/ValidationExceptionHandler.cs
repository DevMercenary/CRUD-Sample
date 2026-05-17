using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CrudSample.Api;

/// <summary>
/// Catches FluentValidation <see cref="ValidationException"/> thrown from
/// services and renders an RFC 7807 ValidationProblemDetails response so
/// the API never leaks a 500 for validation failures.
/// </summary>
public sealed class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException vex)
        {
            return false;
        }

        var errors = vex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var problem = new ValidationProblemDetails(errors)
        {
            Type = "https://crud-sample/errors/validation",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
