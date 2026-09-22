using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        _logger.LogError(
            exception,
            "Unhandled exception. TraceId: {TraceId}",
            traceId);

        var problemDetails = new ProblemDetails
        {
            Status = exception is AppException appException
                ? appException.StatusCode
                : StatusCodes.Status500InternalServerError,

            Title = exception is AppException appException2
                ? appException2.Title
                : "Internal Server Error",

            Detail = exception is AppException appException3
                ? appException3.Message
                : _environment.IsDevelopment()
                    ? exception.Message
                    : "An unexpected error occurred."
        };

        problemDetails.Extensions["traceId"] = traceId;

        if (_environment.IsDevelopment() &&
            exception is not AppException)
        {
            problemDetails.Extensions["exception"] =
                exception.GetType().Name;

            problemDetails.Extensions["stackTrace"] =
                exception.StackTrace;
        }

        httpContext.Response.StatusCode =
            problemDetails.Status!.Value;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}