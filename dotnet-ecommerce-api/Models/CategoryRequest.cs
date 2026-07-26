using System.ComponentModel.DataAnnotations;

namespace dotnet_ecommerce_api.Models
{
    public class CategoryRequest
    {
        [Required, StringLength(150)]
        public string CategoryName { get; set; } = null!;

        [Required]
        public IFormFile File { get; set; } = null!;

    }
}
