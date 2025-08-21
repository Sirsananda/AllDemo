using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllDemo.Models
{
    public class ImageResizerViewModel
    {
        [Display(Name = "Image1")]
        [Required(ErrorMessage = "Image1 is required")]
        public IFormFile? Image { get; set; } = default!;

        [Display(Name = "Image2")]
        [Required(ErrorMessage = "Image2 is Required")]
        public IFormFile? ImageByte { get; set; } = default!;

        [ValidateNever]
        public AfterResizeImage _AfterResizeImage { get; set; }
    }

    public class AfterResizeImage
    {
        public string? LocalDirectoryImage { get;set; }

        public string? ImageStringBase64 { get; set; }

        public byte[]? ImageByte { get; set; }
    }
}
