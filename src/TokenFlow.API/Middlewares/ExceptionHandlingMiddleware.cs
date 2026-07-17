using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using TokenFlow.API.Exceptions;

namespace TokenFlow.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (DomainException ex)
        {
            _logger.LogWarning(ex,
                "Domain rule violation. Message={Message} Path={Path}",
                ex.Message, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = 422,
                Title = "Domain rule violation",
                Detail = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unhandled exception. Message={Message} Path={Path}",
                ex.Message, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = 500,
                Title = "Internal Server Error",
            });
        }
    }
}