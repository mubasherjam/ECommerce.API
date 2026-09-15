using ECommerce.API.DTOs.Cart;

namespace ECommerce.API.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);

        Task<CartItemDto> AddToCartAsync(
            int userId,
            AddToCartDto dto);

        Task<bool> UpdateCartItemAsync(
            int userId,
            int cartItemId,
            UpdateCartItemDto dto);

        Task<bool> RemoveCartItemAsync(
            int userId,
            int cartItemId);
    }
}