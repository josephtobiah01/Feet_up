using System.ComponentModel.DataAnnotations;

namespace FitappAdminWeb.Net7.Models
{
    public class PromotionModel
    {
        [Required]
        public string Icon { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        public string? Url { get; set; }
        [StringLength(250)]
        public string? Message { get; set; }
        public IFormFile? UploadImageFile { get; set; }

        //image parameters
        public byte[]? ImageBytes { get; set; }
        public string? ContentType { get; set; }
    }
}
