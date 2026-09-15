using ECommerce.API.DTOs.Cart;
using ECommerce.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var userId = GetUserId();

            var cart = await _cartService.GetCartAsync(userId);

            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartItemDto>> AddToCart(
            AddToCartDto dto)
        {
            try
            {
                var userId = GetUserId();

                var item = await _cartService.AddToCartAsync(
                    userId,
                    dto);

                return Ok(item);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateCartItem(
            int id,
            UpdateCartItemDto dto)
        {
            try
            {
                var userId = GetUserId();

                var updated =
                    await _cartService.UpdateCartItemAsync(
                        userId,
                        id,
                        dto);

                if (!updated)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> RemoveCartItem(int id)
        {
            var userId = GetUserId();

            var removed =
                await _cartService.RemoveCartItemAsync(
                    userId,
                    id);

            if (!removed)
            {
                return NotFound();
            }

            return NoContent();
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