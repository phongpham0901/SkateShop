using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SkateShop.Models
{
    public class Articlecs
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public string Title { get; set; } = "";

        public string Content { get; set; } = "";

        [MaxLength(100)]
        public string ImageFileName { get; set; } = "";

        public DateTime CreatedAt { get; set; }
    }
}
