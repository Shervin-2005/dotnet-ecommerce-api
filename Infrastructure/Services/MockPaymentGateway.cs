using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class MockPaymentGateway : IPaymentGateway
{
    private const string DeclinedCard = "4000000000000002";
    private const string InsufficientFundsCard = "4000000000009995";
    private const string ExpiredCard = "4000000000000069";

    private readonly ILogger<MockPaymentGateway> _logger;

    public MockPaymentGateway(ILogger<MockPaymentGateway> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentResult> ChargeAsync(PaymentRequest request)
    {
        await Task.Delay(Random.Shared.Next(300, 900));

        var cardNumber = new string(request.CardNumber.Where(char.IsDigit).ToArray());

        var failureReason = cardNumber switch
        {
            DeclinedCard => "Your card was declined.",
            InsufficientFundsCard => "Insufficient funds.",
            ExpiredCard => "Your card has expired.",
            _ when request.Amount % 1 == 0.99m => "Gateway processing error. Please try again.",
            _ => null
        };

        if (failureReason is not null)
        {
            _logger.LogWarning("Mock payment failed for order {OrderId}: {Reason}", request.OrderId, failureReason);
            return new PaymentResult(false, null, failureReason);
        }

        var transactionId = $"mock_txn_{Guid.NewGuid():N}";
        _logger.LogInformation("Mock payment succeeded for order {OrderId}, transaction {TransactionId}",
            request.OrderId, transactionId);

        return new PaymentResult(true, transactionId, null);
    }
}