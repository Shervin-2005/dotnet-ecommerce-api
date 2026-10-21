namespace Application.Interfaces.UserAndAuth
{
    public interface ISmsService
    {
        Task SendAsync(string phoneNumber, string message);
    }
}
