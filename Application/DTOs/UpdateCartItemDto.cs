using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class UpdateCartItemDto
{
    [Range(1, 1000)]
    public int Quantity { get; set; }
}