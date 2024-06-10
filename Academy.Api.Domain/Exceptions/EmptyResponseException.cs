namespace Academy.Api.Domain.Exceptions;

/// <summary>
/// Exception to notify that a webservice's response was empty even though the HTTP Status was not an error
/// </summary>
public class EmptyResponseException : Exception
{
    public EmptyResponseException(string msg = null, Exception inner = null) : base(msg, inner)
    {

    }
}