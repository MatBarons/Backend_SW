using Academy.Api.Domain;
using Academy.Api.Domain.Models.Configuration;
using Academy.Api.Repositories.EF;
using Academy.Api.Repositories.Implementations;
using Academy.Api.Repositories.Interfaces;
using Academy.Api.Services;
using Academy.Api.Web.Filters;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    // Look into NLog.config to see where logs are sent by default. They are sent to console output.
    // If you want to have different Nlog configurations based on environment delete the NLog.config file and reproduce the configuration in appsettings.ENVIRONMENT.json under the NLog key
    // Documentation: https://github.com/NLog/NLog.Extensions.Logging/wiki/NLog-configuration-with-appsettings.json
    
    // PLEASE READ
    // Make sure you have a general idea of the architectural design behind .NET Core by reading this article:
    // https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles
    //
    // This solution heavily uses Dependency Injection design pattern as advised by ASP.NET Core
    // Make sure you understand how it works by reading these documentations:
    // Dependency Injection Design Pattern: https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles#dependency-inversion
    // Dependency Injection in ASP.NET Core: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0

    // SETTINGS & CONFIGURATIONS
    // Official Documentation: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0
    // Any settings of configuration needed by the application can be taken from various places and are all accessed through the IConfiguration object!
    // It is HIGHLY DISCOURAGED, if not deprecated, reading configuration and settings values with other mechanisms, because IConfiguration is natively
    // integrated with many ASP.NET components
    // All values are merged together as they they come from one single source. 
    // By default, this solution natively supports these sources for IConfiguration, merged together in this order:
    // - appsettings.json : appsettings file are json file containing configuration.
    //   - There is one appsettings.json that should contain settings that DO NOT CHANGE BETWEEN ENVIRONMENTS, or default values
    //   - All other settings that change between environments should be placed in appsettings.Development.json, appsettings.Staging.json and appsettings.Production.json. Values wih same keys overwrite values of appsettings.json
    //   - putting secret values in appsettings.json files is DISCOURAGED, you should first consider better options offered by your hosting (Azure KeyVault, AWS Secret Manager, Environment variables)
    // - Environment Variables: any environment variable can be accessed with IConfiguration and can overwrite appsettings.json values (even hierarchical ones using underscores
    // - Command line arguments: any argument passed to the app executable
    // - Many other sources can be added by configuring the configuration object or adding libraries
    // You can control which environment the app is started in by settings the ASPNETCORE_ENVIRONMENT environment variable (Official Documentation: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-6.0)
    //
    // Values in IConfiguration can be made available to all ASP.NET objects using Depencency Injection
    // In particular it is highly encouraged to avoind exposing the IConfiguration object itself but use instead the Options Pattern that binds
    // values in IConfiguration to a custom class you provide. Objects configured in this way are exposed to other objects in Dependency Injection.
    // Please use this method instead of reading directly from IConfiguration.
    // Official Documentation: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0
    // Use this AppSettings object for all the main settings of the app
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(Constants.AppSettingsSectionKeyName));

    builder.Services.AddDistributedMemoryCache();

    //This adds WebApi routing AND tells the router to make all urls lower case
    //Official Documentation: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/routing?view=aspnetcore-6.0
    builder.Services.AddRouting(options => options.LowercaseUrls = true);
    
    builder.Services.AddControllers(options =>
    {
        // We have added a custom filter that allows you to send HTTP responses with statuses different than 200 by throwing an HttpResponseException
        // This is very useful when you need to send data with the response even when you're sending a 5xx or 4xx
        //https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-6.0#use-exceptions-to-modify-the-response
        options.Filters.Add<HttpResponseExceptionFilter>();
    });
    
    // This is an example of implementing Authentication logic
    // For Authentication logic ALWAYS use this mechanism with AuthenticationHandler classes!
    // Official Documentation: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-6.0
    /*
     builder.Services.AddAuthentication("Basic")
        .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null);
    */


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();

    // If you are planning to use OAuth2.0 authentication, this configures Swagger to use it
    /*
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("basic",
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Scheme = "bearer",
                BearerFormat = "JWT"
            }
        );

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "basic"
                    },
                    Scheme = "basic",
                    Name = "basic",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
    });
    */
    builder.Services.AddSwaggerGen();

    // ALWAYS use HttpClient object to make HTTP requests if you need them!
    // Since using multiple HttpClient objects is DEPRECATED you must use this factory object that automatically handles
    // HttpClients and you can access them with Dependency Injection! It comes with configurable timeout
    builder.Services.AddHttpClient(Constants.HttpClientName,
        c => { c.Timeout = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>(Constants.HttpClientTimeoutKey)); });

    // Dependency injection for EF's db context
    // By putting DbContext in DI you don't need to handle manually its lifecycle or the lifecycle of the Sql Connection
    builder.Services.AddDbContext<SqlContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString(Constants.DbConnectionStringKey)));
    
    // All Repositories should be placed under Dependency Injection so they can access objects like Configurations, Settings and Loggers
    // Also this ensures that other objects in DI can access them, like Services
    // It is advised to place them in the Scoped level since they usually hold some sort of state or connection
    builder.Services.AddScoped<IPeopleRepository, PeopleRepository>();

    //In this example we show how to use different implementations of the same repository based on the environment you are in
    /*
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddScoped<IExternalBoWebRepository, ExternalBoWebRepository>();
    }
    else
    {
        builder.Services.AddScoped<IExternalBoWebRepository, ExternalBoWebRepository>();
    }
    */

    // All services should also be places under Dependency Injection to be able to receive repositories, other services and loggers in their constructors
    // Also this is needed to control their lifecycle automatically and pass them to controllers via constructors
    // Usually Services can be put in transient level because Services contain no state
    builder.Services.AddTransient<PeopleService>();
    
    
    builder.Services.Configure<CookiePolicyOptions>(options =>
    {
        options.CheckConsentNeeded = context => true;
    });

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>(Constants.SessionTimeToLiveKeyName));
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

    // This is an example of implementing Authorization in the application
    // It is strongly advised to use Authorization Policies and AuthorizationHandler<>
    // Official Documentation: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-6.0
    /*
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(Constants.Policy.ExistingUser, policy =>
            policy.Requirements.Add(new ExistingUserRequirement()));

        options.DefaultPolicy = options.GetPolicy(Constants.Policy.ExistingUser);
    });
    */
    
    var app = builder.Build();

    // Modern apps should always use HTTPS, but in containerized contexts you may not need it
    if (app.Configuration.GetValue<bool>(Constants.UseHTTPSKeyName, true))
    {
        app.UseHttpsRedirection();
    }

    app.UseExceptionHandler("/error");

    // In case your application is deployed under an URL subpath (e.g. as IIS Application inside a website, or as microservice in k8s)
    // You can tell the routing engine about this subpath so it can correctly know what should be considered as the path base
    //app.UsePathBase(new PathString("/sub/path"));
    
    app.UseRouting();

    if (app.Environment.IsDevelopment())
    {
        // CORS policies for local development when frontend and backend are on different
        app.UseCors(builder => builder
            .AllowCredentials()
            .WithOrigins("http://localhost:5001")
            // TODO change port to match frontend's
            .WithHeaders("Content-Type")
            .WithHeaders(Constants.HeaderAuthorizationKey)
            .AllowAnyMethod());
    }
    
    app.UseSession();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    // Consider if you want to always expose Swagger UI (in case of a REST WebService it may be true) or you just need it for internal usage and tests
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // Adds permalinks to swagger endpoints
            options.EnableDeepLinking();
        });
    }
    
    // If you want Entity Framework to automatically apply any missing migration to your database enable this flag,
    // so any time you start the app it will try to migrate
    if (app.Configuration.GetValue<bool>(Constants.AutoMigrateKey, false))
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            logger.Info("Migrating database schema if needed");
            scope.ServiceProvider.GetRequiredService<SqlContext>().Database.Migrate();
        }
    }

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Fatal(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}