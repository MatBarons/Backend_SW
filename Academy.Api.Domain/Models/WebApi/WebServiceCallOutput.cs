namespace Academy.Api.Domain.Models.WebApi;

/// <summary>
/// Represents the result of a webservice call
/// Contains both the deserialized result of the call and the request arguments 
/// </summary>
public class WebServiceCallOutput<T>
{
    public Uri endpoint { get; set; }
    public object arguments { get; set; }

    public T Result { get; set; }
}