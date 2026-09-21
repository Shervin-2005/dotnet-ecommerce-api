namespace Application.Interfaces;

public record PaymentRequest(
    decimal Amount,
    string CardNumber,
    string CardHolderName,
    string ExpiryMonth,
    string ExpiryYear,
    string Cvv,
    int OrderId);

public record PaymentResult(
    bool IsSuccess,
    string? TransactionId,
    string? FailureReason);

public interface IPaymentGateway
{
    Task<PaymentResult> ChargeAsync(PaymentRequest request);
}