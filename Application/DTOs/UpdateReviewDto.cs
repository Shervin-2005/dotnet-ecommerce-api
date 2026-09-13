using System.ComponentModel.DataAnnotations;
namespace Application.DTOs
{
    public class UpdateReviewDto
    {
        [Range(1, 5)]
        public int Rating { get; set; }
 
        [StringLength(2000)]
        public string? Comment { get; set; }

    }
}

