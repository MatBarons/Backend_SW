using Microsoft.EntityFrameworkCore;

namespace Academy.Api.Repositories.EF;

/// <summary>
/// This is the main context of Entity Framework, basically this is the entrance to the database
/// It was injected in Dependency Injection so you don't need to handle its lifecycle which is automatically handled by ASP.NET
///
/// Entity Framework Core can work both using the Database-First or the Code-First models
/// In Database-First you design the database structure in the database itself and then generate the code here,
/// this can be done with the scaffolding feature of EF: https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/?tabs=dotnet-core-cli
///
/// In Code-First you design the database structure by writing the classes here in code with the DbContext and then you
/// let EF generate the SQL code or directly modify the database.
/// This can be done with Database migrations: https://learn.microsoft.com/en-us/ef/core/managing-schemas/
/// Code-First mode is the recommended way to handle database structure! It is a practice common to many ORMs in many languages
/// and allows the developers to keep the database structure versioned in git and automatically handled by code instead of
/// having the write SQL script by hand to update the database structure.
/// In migrations you can also execute data transformations when your DB structure changes, you can keep track of views
/// definitions and Stored Procedures code.
/// All these features cannot be archieved with Database-First approach, this is why Code-First is recommended.
/// </summary>
public class SqlContext: DbContext
{
    //Every table or view must be linked here with a DbSet to be recognized
    //Remember to keep it with internal visibility so objects of repository don't get discosed to other layers
    internal DbSet<Example> Examples { get; set; }
    internal DbSet<Person> People { get; set; }
    
    private readonly string connectionString = "Server=localhost\\SQLEXPRESS;Database=Test;Trusted_Connection=True;";
    
    public SqlContext()
    {
        
    }
        
    public SqlContext(string connStr): this()
    {
        connectionString = connStr;
    }
        
    public SqlContext(DbContextOptions<SqlContext> options): base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Here you can define properties of the entities and of the tables
        //Documentation: https://learn.microsoft.com/en-us/ef/core/modeling/
        /*
         This is how you define default values for columns
        modelBuilder.Entity<TableClass>()
            .Property(t => t.Column)
            .HasDefaultValue(0);
        */

        /*
         This is how you define a composite key on a table. Composite keys can only be defined here, not with attributes on the table
        modelBuilder.Entity<TableClass>().HasKey(t => new
        {
            t.Key1,
            t.Key2
        });
        */

        //This is how Views are mapped to models
        //The only requirement is to use the [Keyless] attribute on the model class
        //Documentation: https://learn.microsoft.com/en-us/ef/core/modeling/keyless-entity-types?tabs=data-annotations
        //modelBuilder.Entity<ViewClass>().ToView("ViewName", "Schema");
    }
}