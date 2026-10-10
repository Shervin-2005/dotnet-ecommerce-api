using Domain.Enums;

namespace Application.DTOs.Order;

public class OrderDto
{
    public int OrderId { get; set; }

    public OrderStatus Status { get; set; }

    public decimal SubtotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public string RecipientName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string Province { get; set; } = null!;
    public string City { get; set; } = null!;
    public string AddressLine { get; set; } = null!;
    public string PostalCode { get; set; } = null!;

    public List<OrderItemDto> Items { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}