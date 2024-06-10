using Academy.Api.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace Academy.Api.Web.Filters;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    private static Logger logger = LogManager.GetCurrentClassLogger();

    public HttpResponseExceptionFilter()
    {
    }

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is HttpResponseException httpResponseException)
        {
            logger.Error(context.Exception, "HTTP Exception");

            context.Result = new ObjectResult(new ProblemDetails()
            {
                Title = httpResponseException?.ResponseContent?.Title,
                Detail = httpResponseException?.ResponseContent?.Detail,
                Status = httpResponseException?.StatusCode
            })
            {
                StatusCode = httpResponseException?.StatusCode
            };

            context.ExceptionHandled = true;
        }
    }
}