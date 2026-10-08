namespace Domain.Entities;

public class UserAddress
{
    public int UserAddressId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;
    public string RecipientName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Province { get; set; } = null!;
    public string City { get; set; } = null!;
    public string AddressLine { get; set; } = null!;
    public string PostalCode { get; set; } = null!;

    public bool IsDefault { get; set; }

    public User User { get; set; } = null!;
}