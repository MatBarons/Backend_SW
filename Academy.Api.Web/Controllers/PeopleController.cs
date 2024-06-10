using Academy.Api.Domain.Exceptions;
using Academy.Api.Domain.Models.Configuration;
using Academy.Api.Domain.Models.DTOs;
using Academy.Api.Domain.Models.WebApi;
using Academy.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Academy.Api.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    private readonly ILogger<PeopleController> logger;
    private readonly AppSettings appSettings;

    private PeopleService peopleService;

    public PeopleController(ILogger<PeopleController> l, IOptions<AppSettings> appSettings, PeopleService peopleService)
    {
        logger = l;
        this.appSettings = appSettings.Value;
        this.peopleService = peopleService;
    }

    [HttpGet("{name}")]
    public async Task<PersonDto> GetPeopleByName(string name){
        try{
            return await peopleService.GetPeopleByName(name);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error in GetPeopleByName");
            throw new HttpResponseException(StatusCodes.Status500InternalServerError, new ProblemResponse()
            {
                Title = "Failed get",
                Detail = "Getting details for the specified user has failed"
            }, e);
        }
    }

    /*
    [HttpGet("filter")]
    public async Task<List<PersonDto>> getFilteredPeople(string filter){
        try{
            return await peopleService.getFilteredPeople(filter)
        }catch(Exception e){
            
        }
    }
    */
    
}