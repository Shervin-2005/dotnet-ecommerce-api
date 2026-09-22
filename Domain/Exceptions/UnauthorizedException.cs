namespace Domain.Exceptions;

public class UnauthorizedException : AppException
{
    public override int StatusCode => 401;
    public override string Title => "Unauthorized";

    public UnauthorizedException(string message = "Authentication is required or has failed.") : base(message)
    {
    }
}