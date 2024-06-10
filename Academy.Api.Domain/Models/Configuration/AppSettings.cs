namespace Academy.Api.Domain.Models.Configuration;

/// <summary>
/// Model for the main application settings of the App
/// </summary>
public class AppSettings
{
    public int RestTimeout { get; set; }
    
    public bool EnableAutoMigrateDb { get; set; }
}