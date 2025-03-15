using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SkateShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string Brand { get; set; } = "";

        [MaxLength(100)]
        public string Material { get; set; } = "";

        [MaxLength(100)]
        public string Type { get; set; } = "";

        public string Description { get; set; } = "";

        [Precision(16, 2)]
        public decimal Price { get; set; }

        [Precision(16, 2)]
        public decimal OriginalPrice { get; set; }

        [MaxLength(100)]
        public string ImageFileName { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        [MaxLength(100)]
        public string Sale { get; set; } = "";
    }
}
