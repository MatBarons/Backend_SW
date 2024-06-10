using Academy.Api.Domain.Exceptions;
using Academy.Api.Domain.Models.Configuration;
using Academy.Api.Domain.Models.DTOs;
using Academy.Api.Domain.Models.WebApi;
using Academy.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Academy.Api.Web.Controllers;

/// <summary>
/// Controllers are the main entry points of the application, they are considered the higher layer of the solution.
/// Their responsibility is to route user requests down to lower layers to be processed, their purpose is only to be the entrance,
/// no logic should appear in a controller!
/// Other tasks a controller should implement is:
/// * input validation: before passing any input to lower levels it should make at least basic checks for input validity (e.g. input not null, ...)
/// * error handling: any error that should explicitly be handled should be handled in the controller by transforming exceptions to HTTP responses
/// </summary>
//[Authorize] used to enable authorization checks on endpoints
[ApiController]
[Route("[controller]")]
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> logger;
    private readonly AppSettings appSettings;

    private ExampleService exampleService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="l">All controllers should access logging ONLY with ILogger in constructor</param>
    /// <param name="appSettings">Configuration data coming from appsettings, EnvironmentVariables, ecc should be accessed ONLY with this method</param>
    /// <param name="exampleService">Any service that implements Business Logic should be injected in the constructor. IT IS WRONG TO inject repositories!!!</param>
    public ExampleController(ILogger<ExampleController> l, IOptions<AppSettings> appSettings, ExampleService exampleService)
    {
        logger = l;
        this.appSettings = appSettings.Value;
        this.exampleService = exampleService;
    }

    [HttpGet("get_method/{exampleId}")]
    public async Task<ExampleDto> GetExampleMethod(int exampleId)
    {
        //Controller methods have these resposibilities:
        // - Expose logic as HTTP calls
        // - Receive parameters from HTTP requests and VALIDATE them
        // - handle any error or exception coming from Services or lower layers and translate them into HTTP Statuses
        // - DO NOT implement business logic, it must always be handled by Services
        // - Prepare data to be formatted for output, if needed

        try
        {
            if (exampleId <= 0)
            {
                throw new HttpResponseException(StatusCodes.Status400BadRequest, new ProblemResponse()
                {
                    Title = "Bad example id",
                    Detail = "exampleId should be a positive integer"
                });
            }
            
            return await exampleService.GetExampleMethod(exampleId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in get example");
            throw new HttpResponseException(StatusCodes.Status500InternalServerError, new ProblemResponse()
            {
                Title = "Failed get",
                Detail = "Error message for users"
            }, e);
        }
    }
    
    /// <summary>
    /// An example method for deleting an object
    /// In insert/delete/operations do not return any value! Let the HTTP status speak: HTTP 200 = OK, anything else indicates a failure
    /// </summary>
    /// <param name="example"></param>
    /// <exception cref="HttpResponseException"></exception>
    [HttpDelete("del_method")]
    public async Task DeleteExampleMethod([FromBody] ExampleDto example)
    {
        try
        {
            if (example == null)
            {
                throw new HttpResponseException(StatusCodes.Status400BadRequest, new ProblemResponse()
                {
                    Title = "Empty example",
                    Detail = "example parameter must always be specified"
                });
            }
            await exampleService.DeleteExampleMethodAsync(example);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Invalid Operation Exception");
            throw new HttpResponseException(StatusCodes.Status400BadRequest, new ProblemResponse()
            {
                Title = "Example can not be deleted",
                Detail = "Is not possibile to delete example"
            });
        }
    }
}