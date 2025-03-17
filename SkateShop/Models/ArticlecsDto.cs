using System.ComponentModel.DataAnnotations;

namespace SkateShop.Models
{
    public class ArticlecsDto
    {
        [MaxLength(500)]
        public string Title { get; set; } = "";

        public string Content { get; set; } = "";

        public IFormFile? ImageFile { get; set; }
    }
}
