using System.Security.Authentication;
using TouristarModels.Models;
using TouristarBackend.Exceptions;

namespace TouristarBackend.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AppExceptionFilter : ExceptionFilterAttribute
{
    private ILogger _logger;

    public AppExceptionFilter(ILogger<AppExceptionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        string message;
        if (context.Exception.InnerException?.Message != null)
        {
            message = context.Exception.InnerException.Message;
        }
        else
        {
            message = context.Exception.Message;
        }

        _logger.LogError(message, context.Exception.GetType());
        context.Result = GetResult(context.Exception, message);
    }

    private IActionResult? GetResult(Exception exception, string message)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        if (exception is NotFoundException)
        {
            statusCode = StatusCodes.Status404NotFound;
        }
        else if (exception is UserExistsException)
        {
            statusCode = StatusCodes.Status409Conflict;
        }
        else if (exception is AuthenticationException)
        {
            statusCode = StatusCodes.Status401Unauthorized;
        }


        return new ObjectResult(new ErrorResponse
        {
            Message = message,
        })
        {
            StatusCode = statusCode,
        };
    }
}