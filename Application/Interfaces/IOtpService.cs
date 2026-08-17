namespace Application.Interfaces
{
    public interface IOtpService
    {
        Task IssueOtpAsync(string phoneNumber);
        Task<bool> ConsumeOtpAsync(string phoneNumber, string code);
    }
}
