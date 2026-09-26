namespace Application.DTOs.Auth
{
    public class UpdateUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public Stream? Image { get; set; }
        public string? ImageName { get; set; }
        public string? ContentType { get; set; }
    }
}