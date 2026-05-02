using System.Net;
using System.Text.Json;
using WEBArtigos.Common;

namespace WEBArtigos.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exceção não tratada em {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteErrorResponse(context);
        }
    }

    private static async Task WriteErrorResponse(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = ApiResponse<object>.Fail("Ocorreu um erro interno no servidor. Tente novamente mais tarde.");
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
