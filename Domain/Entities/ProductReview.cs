namespace Domain.Entities;

public class ProductReview
{
    public int ProductReviewId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public int Rating { get; set; }

    public string Comment { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    
    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
}