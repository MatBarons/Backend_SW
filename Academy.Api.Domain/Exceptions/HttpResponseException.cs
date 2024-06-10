using Academy.Api.Domain.Models.WebApi;

namespace Academy.Api.Domain.Exceptions;

/// <summary>
/// Exception to be used to trigger an HTTP response different than 200 from any layer of the application
/// </summary>
public class HttpResponseException : Exception
{
    public int StatusCode { get; }
    public ProblemResponse? ResponseContent { get; }

    public HttpResponseException(int statusCode = 500, ProblemResponse? response = null, Exception? inner = null)
    : base(response?.Detail, inner)
    {
        StatusCode = statusCode;
        ResponseContent = response;
    }
}