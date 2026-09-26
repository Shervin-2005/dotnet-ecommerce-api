using Application.DTOs.Cart;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartActionResult> AddItemAsync(int userId, AddToCartDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
        if (product is null) return CartActionResult.ProductNotFound;

        var cart = await GetOrCreateCartAsync(userId);

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        var requestedTotalQuantity = (existingItem?.Quantity ?? 0) + dto.Quantity;

        if (requestedTotalQuantity > product.StockQuantity)
            return CartActionResult.OutOfStock;

        if (existingItem is not null)
        {
            existingItem.Quantity = requestedTotalQuantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.CartId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };
            await _unitOfWork.CartItems.AddAsync(newItem);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return CartActionResult.Success;
    }

    public async Task<CartActionResult> UpdateQuantityAsync(int userId, int cartItemId, UpdateCartItemDto dto)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
        var item = cart?.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
        if (item is null) return CartActionResult.ItemNotFound;

        if (dto.Quantity > item.Product.StockQuantity)
            return CartActionResult.OutOfStock;

        item.Quantity = dto.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return CartActionResult.Success;
    }

    public async Task<CartActionResult> RemoveItemAsync(int userId, int cartItemId)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
        var item = cart?.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
        if (item is null) return CartActionResult.ItemNotFound;

        _unitOfWork.CartItems.Delete(item);
        await _unitOfWork.SaveChangesAsync();
        return CartActionResult.Success;
    }

    public async Task ClearCartAsync(int userId)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
        if (cart is null || cart.Items.Count == 0) return;

        foreach (var item in cart.Items.ToList())
            _unitOfWork.CartItems.Delete(item);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Cart> GetOrCreateCartAsync(int userId)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId);
        if (cart is not null) return cart;

        cart = new Cart { UserId = userId };
        await _unitOfWork.Carts.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync(); 

        return cart;
    }
}