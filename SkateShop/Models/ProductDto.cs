using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SkateShop.Models
{
    public class ProductDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required, MaxLength(100)]
        public string Brand { get; set; } = "";

        [Required, MaxLength(100)]
        public string Material { get; set; } = "";

        [Required, MaxLength(100)]
        public string Size { get; set; } = "";

        [Required, MaxLength(100)]
        public string Type { get; set; } = "";

        [Required]
        public decimal Price { get; set; }

        [Precision(16, 2)]
        public decimal OriginalPrice { get; set; }

        public IFormFile? ImageFile { get; set; }

        [MaxLength(100)]
        public string Sale { get; set; } = "";
    }
}
