using System.Net;
using System.Text.Json;
using InventoryManagement.Application.Common.Exceptions;

namespace InventoryManagement.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = ResolveStatusCode(exception);

        if (statusCode == (int)HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception on {Path}", context.Request.Path);
        }
        else
        {
            _logger.LogWarning("Request to {Path} failed: {Message}", context.Request.Path, exception.Message);
        }

        var response = new ErrorResponse(statusCode, ResolveMessage(exception, statusCode));

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }

    private static int ResolveStatusCode(Exception exception) => exception switch
    {
        NotFoundException => (int)HttpStatusCode.NotFound,
        ConflictException => (int)HttpStatusCode.Conflict,
        _ => (int)HttpStatusCode.InternalServerError
    };

    private static string ResolveMessage(Exception exception, int statusCode) =>
        statusCode == (int)HttpStatusCode.InternalServerError
            ? "An unexpected error occurred. Please try again later."
            : exception.Message;
}