using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class UpdateCategoryDto
    {
        [Required, StringLength(150)]
        public string CategoryName { get; set; } = null!;

        [Required]
        public Stream Image { get; set; } = default!;

        [Required]
        public string ImageName { get; set; } = null!;

        [Required]
        public string ContentType { get; set; } = null!;
    }
}
