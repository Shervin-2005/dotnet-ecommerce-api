using Application.DTOs.Cart;
using Domain.Enums;

namespace Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(int userId);
    Task<CartActionResult> AddItemAsync(int userId, AddToCartDto dto);
    Task<CartActionResult> UpdateQuantityAsync(int userId, int cartItemId, UpdateCartItemDto dto);
    Task<CartActionResult> RemoveItemAsync(int userId, int cartItemId);
    Task ClearCartAsync(int userId);
}