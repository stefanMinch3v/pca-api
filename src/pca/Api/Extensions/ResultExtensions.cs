using pca.Application.Common;

namespace pca.Api.Extensions;

/// <summary>
/// Maps a failed Application-layer <see cref="Result"/>/<see cref="Result{TData}"/>
/// onto an RFC 9457 ProblemDetails response. The Application layer only
/// knows Success/Failure - deciding what that means over HTTP (which status
/// code, which shape) belongs here, not there.
/// </summary>
public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.Succeeded)
        {
            throw new InvalidOperationException("Can't convert a result to a problem response.");
        }

        return Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Bad Request",
            extensions: new Dictionary<string, object?>
            {
                ["errors"] = result.Error.Messages,
            });
    }
}
