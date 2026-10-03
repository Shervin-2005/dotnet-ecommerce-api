namespace Application.DTOs.Product
{
    public class ProductImageUploadDto
    {
        public Stream Image { get; set; } = null!;
        public string ImageName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        public int DisplayOrder { get; set; }
    }
}
