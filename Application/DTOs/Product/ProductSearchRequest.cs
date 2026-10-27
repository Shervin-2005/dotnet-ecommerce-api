namespace Application.DTOs.Product;

public class ProductSearchRequest
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public int? BrandId { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }
    
    public string SortBy { get; set; } = "relevance";
    
    public string SortOrder { get; set; } = "asc";

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}