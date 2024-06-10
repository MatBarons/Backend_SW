namespace Academy.Api.Domain;

/// <summary>
/// Class for all constants in the project
/// </summary>
public class Constants
{
    public const string UseHTTPSKeyName = "UseHTTPS";
    
    public const string HttpClientName = "ClientWithTimeout";
    public const string HttpClientTimeoutKey = "AppSettings:RestTimeoutSeconds";

    public const string SessionTimeToLiveKeyName = "AppSettings:SessionTimeToLiveMinutes";
    
    public const string AppSettingsSectionKeyName = "AppSettings";
    
    public const string HeaderAuthorizationKey = "Authorization";

    public const string DbConnectionStringKey = "Sql";
    public const string AutoMigrateKey = "AppSettings:EnableAutoMigrateDb";
}