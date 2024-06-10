using Microsoft.EntityFrameworkCore.Design;

namespace Academy.Api.Repositories.EF;

/// <summary>
/// Factory used to read the connection string from an Environment Variable.
/// NOT USED IN ACTUAL CODE
/// It is used only for dotnet CLI Tools in order to setup the connection string for migrations
/// </summary>
public class ContextFactory: IDesignTimeDbContextFactory<SqlContext>
{
    public ContextFactory()
    {
        
    }
    
    public SqlContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("EFCORETOOLSDB");
        /*if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("The connection string was not set " +
                                                "in the 'EFCORETOOLSDB' environment variable.");*/
            
        return new SqlContext(connectionString);
    }
}