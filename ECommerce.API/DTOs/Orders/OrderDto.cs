namespace ECommerce.API.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public List<OrderItemDto> Items { get; set; }
            = new List<OrderItemDto>();
    }
}