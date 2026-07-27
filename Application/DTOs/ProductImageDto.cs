namespace Application.DTOs
{
    public class ProductImageDto
    {
        public Stream Image { get; set; } = null!;
        public string ImageName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
    }
}
