using System.Text.Json;
using Veloce.Exceptions;

namespace Veloce.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException e)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = e.StatusCode;
            var response = new { message = e.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception e)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            var response = new { message = e.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}