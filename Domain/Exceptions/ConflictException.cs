namespace Domain.Exceptions;

public class ConflictException : AppException
{
    public override int StatusCode => 409;
    public override string Title => "Conflict";

    public ConflictException(string message) : base(message)
    {
    }
}