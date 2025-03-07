using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkateShop.Models
{
    [Table("OrderItems")]
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        [Precision(16, 2)]
        public decimal UnitPrice { get; set; }

        // navigation property
        public Product Product { get; set; } = new Product();

        // Khóa ngoại kết nối với Order
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!; // Navigation property
    }
}
