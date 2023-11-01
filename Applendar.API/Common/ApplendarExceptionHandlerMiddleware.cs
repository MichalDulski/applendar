using System.Net;
using System.Text.Json;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Applendar.API.Common;

public class ApplendarExceptionHandlerMiddleware
{
    private static readonly ILogger Logger = Log.ForContext<ApplendarExceptionHandlerMiddleware>();

    private readonly RequestDelegate _next;
    
    public (HttpStatusCode code, string message) GetResponse(Exception exception)
    {
        HttpStatusCode code;
        switch (exception)
        {
            case KeyNotFoundException
                 or FileNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                break;
            case ArgumentException
                 or InvalidOperationException:
                code = HttpStatusCode.BadRequest;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                break;
        }
        return (code, JsonSerializer.Serialize(new ErrorResponse(exception.Message)));
    }    
    
    public ApplendarExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            // log the error
            Logger.Error(exception, "error during executing {Context}", context.Request.Path.Value);
            var response = context.Response;
            response.ContentType = "application/json";
            
            // get the response code and message
            var (status, message) = GetResponse(exception);
            response.StatusCode = (int) status;
            await response.WriteAsync(message);
        }
    }
}

public record ErrorResponse(string Message);