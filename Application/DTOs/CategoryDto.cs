using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        [Required, StringLength(150)]
        public string CategoryName { get; set; } = null!;

        [StringLength(500)]
        public string? MainImageUrl { get; set; }
    }
}
