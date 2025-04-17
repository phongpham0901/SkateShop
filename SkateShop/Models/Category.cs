using System.ComponentModel.DataAnnotations;

namespace SkateShop.Models
{
    public class Category
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public string Name { get; set; } = "";
        public string Des { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
