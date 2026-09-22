using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller
{
    [Authorize]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<OrderDto>> Checkout(CreateOrderDto dto)
        {
            var order = await _orderService.CheckoutAsync(GetUserId(), dto);
            return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            var orders = await _orderService.GetMyOrdersAsync(GetUserId());
            return Ok(orders);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var isAdmin = User.IsInRole("Admin");
            var (result, order) = await _orderService.GetOrderDetailAsync(id, GetUserId(), isAdmin);

            return result switch
            {
                OrderActionResult.Success => Ok(order),
                OrderActionResult.NotFound => NotFound(),
                OrderActionResult.Forbidden => Forbid(),
                _ => BadRequest()
            };
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateStatusAsync(id, dto);
            return result switch
            {
                OrderActionResult.Success => NoContent(),
                OrderActionResult.NotFound => NotFound(),
                OrderActionResult.InvalidStatusTransition =>
                    Conflict("That status change isn't allowed from the order's current status."),
                _ => BadRequest()
            };
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _orderService.CancelAsync(id, GetUserId());
            return result switch
            {
                OrderActionResult.Success => NoContent(),
                OrderActionResult.NotFound => NotFound(),
                OrderActionResult.Forbidden => Forbid(),
                OrderActionResult.InvalidStatusTransition => Conflict("This order can no longer be cancelled."),
                _ => BadRequest()
            };
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}