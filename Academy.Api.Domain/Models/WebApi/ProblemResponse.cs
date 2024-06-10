namespace Academy.Api.Domain.Models.WebApi;

/// <summary>
/// Model to be used as content of an HttpResponseException
/// </summary>
public class ProblemResponse
{
    public string Title { get; set; } = "Unexpected Error";

    public string Detail { get; set; }
}