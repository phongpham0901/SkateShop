using System.ComponentModel.DataAnnotations;

namespace SkateShop.Models
{
    public class Assess
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int start { get; set; }

        [Required]
        public string Description { get; set; }

        public Product Product { get; set; } = new Product();
    }
}
