using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs.Cart
{
    public class UpdateCartItemDto
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}