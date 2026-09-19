using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs;

public class UpdateOrderStatusDto
{
    [Required]
    public OrderStatus Status { get; set; }
}