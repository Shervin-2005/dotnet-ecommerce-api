namespace dotnet_ecommerce_api.Models;

public class UserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public IFormFile? File { get; set; }
}