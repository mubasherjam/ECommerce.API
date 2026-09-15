using ECommerce.API.Data;
using ECommerce.API.DTOs.Orders;
using ECommerce.API.Models;
using ECommerce.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto> CreateOrderAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new ArgumentException(
                    "Your cart is empty.");
            }

            // Check stock
            foreach (var item in cart.CartItems)
            {
                if (item.Quantity > item.Product.StockQuantity)
                {
                    throw new ArgumentException(
                        $"Not enough stock for {item.Product.Name}.");
                }
            }

            // Calculate total
            var total = cart.CartItems.Sum(item =>
                item.Product.Price * item.Quantity);

            // Create order
            var order = new Order
            {
                UserId = userId,
                TotalAmount = total,
                Status = "Pending",
                OrderDate = DateTime.UtcNow
            };

            // Create order items
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price
                };

                order.OrderItems.Add(orderItem);

                // Reduce stock
                cartItem.Product.StockQuantity -= cartItem.Quantity;
            }

            _context.Orders.Add(order);

            // Remove cart items after checkout
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            return (await GetOrderByIdAsync(userId, order.Id))!;
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(
            int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    OrderDate = o.OrderDate,

                    Items = o.OrderItems
                        .Select(item => new OrderItemDto
                        {
                            ProductId = item.ProductId,
                            ProductName = item.Product.Name,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            Total = item.UnitPrice * item.Quantity
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(
            int userId,
            int orderId)
        {
            return await _context.Orders
                .Where(o =>
                    o.Id == orderId &&
                    o.UserId == userId)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    OrderDate = o.OrderDate,

                    Items = o.OrderItems
                        .Select(item => new OrderItemDto
                        {
                            ProductId = item.ProductId,
                            ProductName = item.Product.Name,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            Total = item.UnitPrice * item.Quantity
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}