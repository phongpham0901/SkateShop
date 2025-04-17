using System.ComponentModel.DataAnnotations;

namespace SkateShop.Models
{
    public class CategoryDto
    {
        [MaxLength(500)]
        public string Name { get; set; } = "";

        public string Des { get; set; } = "";
    }
}
