using AppCore.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace WebApi;

public class ProblemDetailsExceptionHandler(
    ProblemDetailsFactory factory,
    ILogger<ProblemDetailsExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ContactNotFoundException)
        {
            logger.LogInformation("Exception '{Message}' handled!", exception.Message);

            var problem = factory.CreateProblemDetails(
                context,
                StatusCodes.Status400BadRequest,
                "Contact service error!",
                "Service error",
                detail: exception.Message
            );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(problem);
            return true;
        }

        logger.LogError(exception, "Unhandled exception");

        var genericProblem = factory.CreateProblemDetails(
            context,
            StatusCodes.Status500InternalServerError,
            "Unhandled server error",
            "Server error",
            detail: exception.ToString()
        );

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(genericProblem);
        return true;
    }
}