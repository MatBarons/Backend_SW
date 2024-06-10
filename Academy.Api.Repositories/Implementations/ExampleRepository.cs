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
public class ExampleRepository : IExampleRepository
{
    private SqlContext dbContext;
    
    public ExampleRepository(SqlContext ctx)
    {
        dbContext = ctx;
    }
    
    /// <summary>
    /// Example of deleting record from a table
    /// Documentation: https://learn.microsoft.com/en-us/ef/core/saving/
    /// </summary>
    /// <param name="exampleId"></param>
    public async Task DeleteExampleMethod(int exampleId)
    {
        await dbContext.Examples.Where(e => e.Id == exampleId).ExecuteDeleteAsync();
    }

    /// <summary>
    /// Example of retrieving a record from a table by using LINQ methods
    /// Since EF exposes tables as IEnumerables objects you can use LINQ to make queries to the database
    /// LINQ syntax gets automatically translated to SQL Code by LinqToSQL
    ///
    /// Documentation: https://learn.microsoft.com/en-us/ef/core/querying/
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ExampleDto? GetExample(int id)
    {
        return dbContext.Examples.Where(e => e.Id == id)
            .Select(e => new ExampleDto()
            {
                ExampleId = e.Id,
                ExampleName = e.Name
            }).FirstOrDefault();
    }

    /// <summary>
    /// Example of retrieving multiple records from a table by using LINQ sql-like syntax
    /// This syntax is not limited to usage with database, it is a general LINQ syntax so you can use it with any
    /// IEnumerable (like lists, dictionaries, ecc)
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public List<ExampleDto> GetExamplesByName(string name)
    {
        return (from ex in dbContext.Examples
            where ex.Name.StartsWith(name)
            select new ExampleDto()
            {
                ExampleId = ex.Id,
                ExampleName = ex.Name
            }).ToList();
    }
}