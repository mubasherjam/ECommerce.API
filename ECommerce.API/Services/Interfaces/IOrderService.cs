using ECommerce.API.DTOs.Orders;

namespace ECommerce.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(int userId);

        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int userId);

        Task<OrderDto?> GetOrderByIdAsync(
            int userId,
            int orderId);

        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

        Task<bool> UpdateOrderStatusAsync(
            int orderId,
            string status);
    }
}