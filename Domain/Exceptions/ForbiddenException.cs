namespace Domain.Exceptions;

public class ForbiddenException : AppException
{
    public override int StatusCode => 403;
    public override string Title => "Forbidden";

    public ForbiddenException(string message = "You don't have permission to perform this action.") : base(message)
    {
    }
}