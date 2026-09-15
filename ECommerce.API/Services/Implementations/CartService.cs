using ECommerce.API.Data;
using ECommerce.API.DTOs.Cart;
using ECommerce.API.Models;
using ECommerce.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var items = await _context.CartItems
                .Where(x => x.CartId == cart.Id)
                .Include(x => x.Product)
                .Select(x => new CartItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    Price = x.Product.Price,
                    Quantity = x.Quantity,
                    Total = x.Product.Price * x.Quantity
                })
                .ToListAsync();

            return new CartDto
            {
                Id = cart.Id,
                Items = items,
                Total = items.Sum(x => x.Total)
            };
        }

        public async Task<CartItemDto> AddToCartAsync(
            int userId,
            AddToCartDto dto)
        {
            var product = await _context.Products
                .FindAsync(dto.ProductId);

            if (product == null)
            {
                throw new ArgumentException(
                    "Product does not exist.");
            }

            if (product.StockQuantity < dto.Quantity)
            {
                throw new ArgumentException(
                    "Not enough stock available.");
            }

            var cart = await GetOrCreateCartAsync(userId);

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.CartId == cart.Id &&
                    x.ProductId == dto.ProductId);

            if (existingItem != null)
            {
                var newQuantity =
                    existingItem.Quantity + dto.Quantity;

                if (newQuantity > product.StockQuantity)
                {
                    throw new ArgumentException(
                        "Not enough stock available.");
                }

                existingItem.Quantity = newQuantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };

                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            var item = await _context.CartItems
                .Where(x =>
                    x.CartId == cart.Id &&
                    x.ProductId == dto.ProductId)
                .Include(x => x.Product)
                .Select(x => new CartItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name,
                    Price = x.Product.Price,
                    Quantity = x.Quantity,
                    Total = x.Product.Price * x.Quantity
                })
                .FirstAsync();

            return item;
        }

        public async Task<bool> UpdateCartItemAsync(
            int userId,
            int cartItemId,
            UpdateCartItemDto dto)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var item = await _context.CartItems
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.Id == cartItemId &&
                    x.CartId == cart.Id);

            if (item == null)
            {
                return false;
            }

            if (item.Product.StockQuantity < dto.Quantity)
            {
                throw new ArgumentException(
                    "Not enough stock available.");
            }

            item.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveCartItemAsync(
            int userId,
            int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.Id == cartItemId &&
                    x.CartId == cart.Id);

            if (item == null)
            {
                return false;
            }

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (cart != null)
            {
                return cart;
            }

            cart = new Cart
            {
                UserId = userId
            };

            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();

            return cart;
        }
    }
}