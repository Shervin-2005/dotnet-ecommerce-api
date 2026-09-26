namespace Application.DTOs.Payment;

public class PayOrderDto
{
    public string CardNumber { get; set; } = null!;
    public string CardHolderName { get; set; } = null!;
    public string ExpiryMonth { get; set; } = null!;
    public string ExpiryYear { get; set; } = null!;
    public string Cvv { get; set; } = null!;
}