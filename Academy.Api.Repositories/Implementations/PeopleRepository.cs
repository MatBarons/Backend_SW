using Academy.Api.Domain.Models.DTOs;
using Academy.Api.Repositories.EF;
using Academy.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Academy.Api.Repositories.Implementations;

/// <summary>
/// Actual implementation of a IExampleRepository
///
/// Repositories abstract interaction with any external resource or service (e.g. Databases, Cache, Storage, FileSystem, WebServices, Cloud Services, ...)
/// They must be the only components that are aware the exact nature of an external resource, it should not expose details to other components about what the actual external resource is
///
/// Repositories should ONLY implement logic that deals with the exact nature of the external resource (i.e. how to read or write a file, how to query a table in a DB, ...)
/// Repositories should NEVER implement application logic, it is the duty of a Service to do so
/// Repositories should expose methods that could be implemented by another repository that provides the same data but with a different technology (e.g. cache instead of database, database instead of file, ...)
/// Repositories must always implement an Interface, so it can be used in Dependency Injection and can be easily swapped with another repository implementing the same data
/// </summary>
public class PeopleRepository : IPeopleRepository
{
    private SqlContext dbContext;
    
    public PeopleRepository(SqlContext ctx)
    {
        dbContext = ctx;
    }

    public PersonDto? GetPeopleByName(string name)
    {
        return dbContext.People.Where(e => e.Name == name)
            .Select(e => new PersonDto(){
                Name = e.Name
                
            }).FirstOrDefault();
    }
}