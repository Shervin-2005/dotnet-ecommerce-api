namespace Domain.Exceptions;

public class BadRequestException : AppException
{
    public override int StatusCode => 400;
    public override string Title => "Invalid request";

    public BadRequestException(string message) : base(message)
    {
    }
}