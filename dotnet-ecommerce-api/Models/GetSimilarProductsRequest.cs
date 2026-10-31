namespace dotnet_ecommerce_api.Models;

public class GetSimilarProductsRequest
{
    public int ProductId { get; set; }
    public int Count { get; set; } = 8;
}