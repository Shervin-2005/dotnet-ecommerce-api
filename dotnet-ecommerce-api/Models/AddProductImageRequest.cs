using Microsoft.AspNetCore.Http;

namespace dotnet_ecommerce_api.Models;

public class AddProductImageRequest
{
    public IFormFile File { get; set; } = null!;

    public bool IsMain { get; set; }

    public int DisplayOrder { get; set; }
}