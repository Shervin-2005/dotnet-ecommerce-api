using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class PayOrderDto
{
    [Required]
    [CreditCard]
    public string CardNumber { get; set; } = null!;

    [Required, StringLength(100)]
    public string CardHolderName { get; set; } = null!;

    [Required, RegularExpression(@"^(0[1-9]|1[0-2])$", ErrorMessage = "Expiry month must be 01-12.")]
    public string ExpiryMonth { get; set; } = null!;

    [Required, RegularExpression(@"^\d{4}$", ErrorMessage = "Expiry year must be 4 digits.")]
    public string ExpiryYear { get; set; } = null!;

    [Required, RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
    public string Cvv { get; set; } = null!;
}