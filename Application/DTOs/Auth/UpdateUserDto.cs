using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }

        [Required]
        public Stream Image { get; set; } = default!;

        [Required]
        public string ImageName { get; set; } = null!;

        [Required]
        public string ContentType { get; set; } = null!;
    }
}
