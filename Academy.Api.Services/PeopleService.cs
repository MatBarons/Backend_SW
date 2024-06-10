using Academy.Api.Domain.Models.DTOs;
using Academy.Api.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Academy.Api.Services;

/// <summary>
/// Service implementing the business logic to deal with example
///
/// Services are responsible to implementing all the logic of the application.
/// All data manipulation, transformation, calling different data sources ecc should be done in a service
/// 
/// They are invoked by controllers or other services and they may use repositories to interact with external systems (database, WebServices, ecc)
///
/// Services MUST be:
/// - DataSource agnostic: they MUST NOT know anything about the nature of a source, because it is the responsibility of
///     a Repository to know it. If your service works by knowing that a data source is a database or a webservice then it is wrong
/// - Output agnostic: they MUST NOT know where their output is going. Returned data of a service may go to a Web Controller,
///     or to another Service, or to a Presentation layer of a Console Application; so it must never assume where it data is going
///     (e.g. never throw an HTTP error in a service, it the duty of a Web Controller to trasform an Exception to an HTTP Status)
/// </summary>
public class PeopleService
{
    private static ILogger<PeopleService> logger;
    private readonly IPeopleRepository peopleRepository;

    public PeopleService (IPeopleRepository peopleRepository, ILogger<PeopleService> l)
    {
        this.peopleRepository = peopleRepository;
        logger = l;
    }

    public async Task<PersonDto> GetPeopleByName(string name){
        return await Task.FromResult(peopleRepository.GetPeopleByName(name));
    }
}