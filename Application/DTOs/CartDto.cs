namespace Application.DTOs;

public class CartDto
{
    public int CartId { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public decimal TotalPrice { get; set; }
}