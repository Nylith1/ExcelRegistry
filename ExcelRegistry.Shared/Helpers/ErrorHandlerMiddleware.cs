using ExcelRegistry.Shared.Helpers.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ExcelRegistry.Shared.Helpers;

public class ErrorHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotAuthorizedException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { Success = false, Message = "Unauthorized access." });
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Success = false, Message = $"{ex.Message}" });
        }
        catch
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { Success = false, Message = "An internal error occurred while processing request." });
        }
    }
}
