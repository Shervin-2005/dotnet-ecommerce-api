using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class UpdateCategoryDto
    {
        [Required, StringLength(150)]
        public string CategoryName { get; set; } = null!;

        [Required, StringLength(500)]
        public string MainImageUrl { get; set; } = null!;
    }
}
