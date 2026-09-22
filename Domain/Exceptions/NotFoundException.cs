namespace Domain.Exceptions;

public class NotFoundException : AppException
{
    public override int StatusCode => 404;
    public override string Title => "Resource not found";

    public NotFoundException(string message = "The requested resource was not found.") : base(message)
    {
    }
}