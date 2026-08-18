using System.ComponentModel.DataAnnotations;

namespace dotnet_ecommerce_api.Models
{
    public class UserRequest
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
