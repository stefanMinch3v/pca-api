using Microsoft.AspNetCore.Diagnostics;

namespace pca.Api;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(
            exception,
            "Unhandled exception while processing {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        var errors = environment.IsDevelopment()
            ? new[] { exception.Message }
            : new[] { "An unexpected error occurred. Please try again later." };

        await Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An unexpected error occurred.",
                extensions: new Dictionary<string, object?> { ["errors"] = errors })
            .ExecuteAsync(httpContext);

        return true;
    }
}
