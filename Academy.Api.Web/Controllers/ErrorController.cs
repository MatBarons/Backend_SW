using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Api.Web.Controllers;

/// <summary>
/// This Controller catches all uncaught errors and makes sure they are correctly shown
/// </summary>
[ApiController]
[Route("[controller]")]
public class ErrorController : ControllerBase
{
    private ILogger<ErrorController> logger;

    public ErrorController(ILogger<ErrorController> l)
    {
        logger = l;
    }
    
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("")]
    public IActionResult HandleError([FromServices] IHostEnvironment hostEnvironment)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            return Problem(
                title: "An error occurred",
                detail: "An unexpected error occurred, your request cannot be processed");
        }

        var exceptionHandlerFeature =
            HttpContext.Features.Get<IExceptionHandlerFeature>()!;

        logger.LogError(exceptionHandlerFeature.Error, "Internal Server Error");

        return Problem(
            detail: "Internal Server Error",
            title: "Internal Server Error",
            statusCode: 500);
    }
}