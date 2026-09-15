using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs.Orders
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}