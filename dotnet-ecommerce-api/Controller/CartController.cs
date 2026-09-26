using System.Security.Claims;
using Application.DTOs.Cart;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller
{
    [Authorize]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var cart = await _cartService.GetCartAsync(GetUserId());
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem(AddToCartDto dto)
        {
            var result = await _cartService.AddItemAsync(GetUserId(), dto);

            return result switch
            {
                CartActionResult.Success => NoContent(),
                CartActionResult.ProductNotFound => NotFound("Product not found."),
                CartActionResult.OutOfStock => Conflict("Not enough stock available."),
                _ => BadRequest()
            };
        }

        [HttpPut("items/{cartItemId:int}")]
        public async Task<IActionResult> UpdateQuantity(
            int cartItemId,
            UpdateCartItemDto dto)
        {
            var result = await _cartService.UpdateQuantityAsync(
                GetUserId(),
                cartItemId,
                dto);

            return result switch
            {
                CartActionResult.Success => NoContent(),
                CartActionResult.ItemNotFound => NotFound("Cart item not found."),
                CartActionResult.OutOfStock => Conflict("Not enough stock available."),
                _ => BadRequest()
            };
        }

        [HttpDelete("items/{cartItemId:int}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var result = await _cartService.RemoveItemAsync(
                GetUserId(),
                cartItemId);

            return result switch
            {
                CartActionResult.Success => NoContent(),
                CartActionResult.ItemNotFound => NotFound("Cart item not found."),
                _ => BadRequest()
            };
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync(GetUserId());

            return NoContent();
        }

        private int GetUserId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}