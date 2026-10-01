using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Jobs;

public class OtpCleanupJob : IOtpCleanupJob
{
    private readonly IOtpService _otpService;
    private readonly ILogger<OtpCleanupJob> _logger;

    public OtpCleanupJob(
        IOtpService otpService,
        ILogger<OtpCleanupJob> logger)
    {
        _otpService = otpService;
        _logger = logger;
    }

    public async Task DeleteExpiredOtpsAsync()
    {
        _logger.LogInformation("Starting expired OTP cleanup.");

        await _otpService.DeleteExpiredOtpsAsync();

        _logger.LogInformation("Expired OTP cleanup completed.");
    }
}