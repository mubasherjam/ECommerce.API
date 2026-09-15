using ECommerce.API.DTOs.Orders;
using ECommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/orders
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder()
        {
            try
            {
                var userId = GetUserId();

                var order =
                    await _orderService.CreateOrderAsync(userId);

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/orders/admin/all
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            return Ok(orders);
        }

        // PUT: api/orders/admin/1/status
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int id,
            UpdateOrderStatusDto dto)
        {
            var validStatuses = new[]
            {
        "Pending",
        "Processing",
        "Shipped",
        "Delivered",
        "Cancelled"
    };

            if (!validStatuses.Contains(dto.Status))
            {
                return BadRequest(
                    "Invalid order status.");
            }

            var updated = await _orderService
                .UpdateOrderStatusAsync(id, dto.Status);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }
        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {

            var userId = GetUserId();

            var orders =
                await _orderService.GetUserOrdersAsync(userId);

            return Ok(orders);
        }

        // GET: api/orders/1
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var userId = GetUserId();

            var order =
                await _orderService.GetOrderByIdAsync(
                    userId,
                    id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        private int GetUserId()
        {
            return int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!
            );
        }
    }
}