using System.ComponentModel.DataAnnotations;

namespace SkateShop.Models
{
    public class AssessDto
    {
        [Required]
        public int start { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}
