namespace Academy.Api.Domain.Exceptions;
public class FieldsRequiredException : Exception
{
    public FieldsRequiredException(string msg = null, Exception inner = null) : base(msg, inner)
    {

    }
}