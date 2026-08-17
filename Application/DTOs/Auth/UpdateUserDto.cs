using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth
{
    public class UpdateUserDto
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }

        [MinLength(8), MaxLength(12)]
        public string? Password { get; set; }
    }
}
