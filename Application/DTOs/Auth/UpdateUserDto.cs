using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }

        public Stream? Image { get; set; } 

        public string? ImageName { get; set; }

        public string? ContentType { get; set; }
    }
}
