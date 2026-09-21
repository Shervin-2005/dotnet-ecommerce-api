using System.Security.Claims;
using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_ecommerce_api.Controller
{
    [Authorize]
    [Route("api/orders/{orderId:int}/payments")]
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<PaymentDto>> Pay(int orderId, PayOrderDto dto)
        {
            var (result, payment) = await _paymentService.PayOrderAsync(orderId, GetUserId(), dto);

            return result switch
            {
                PaymentActionResult.Success => Ok(payment),
                PaymentActionResult.OrderNotFound => NotFound(),
                PaymentActionResult.Forbidden => Forbid(),
                PaymentActionResult.OrderNotPayable => Conflict("This order can't be paid in its current status."),
                PaymentActionResult.PaymentDeclined => StatusCode(StatusCodes.Status402PaymentRequired, payment),
                _ => BadRequest()
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments(int orderId)
        {
            var isAdmin = User.IsInRole("Admin");
            var (result, payments) = await _paymentService.GetOrderPaymentsAsync(orderId, GetUserId(), isAdmin);

            return result switch
            {
                PaymentActionResult.Success => Ok(payments),
                PaymentActionResult.OrderNotFound => NotFound(),
                PaymentActionResult.Forbidden => Forbid(),
                _ => BadRequest()
            };
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}