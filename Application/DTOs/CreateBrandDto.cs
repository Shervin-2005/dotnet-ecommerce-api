using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CreateBrandDto
    {
        [Required, StringLength(150)]
        public string BrandName { get; set; } = null!;

        [Required, StringLength(500)]
        public string MainImageUrl { get; set; } = null!;
    }
}
