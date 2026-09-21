namespace Domain.Enums;

public enum PaymentActionResult
{
    Success,
    OrderNotFound,
    Forbidden,
    OrderNotPayable,
    PaymentDeclined
}