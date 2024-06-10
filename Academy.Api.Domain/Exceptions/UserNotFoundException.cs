namespace Academy.Api.Domain.Exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string? msg = null, Exception? inner = null) : base(msg, inner)
    { }
}
