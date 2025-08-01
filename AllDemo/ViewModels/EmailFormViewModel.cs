using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AllDemo.ViewModels
{
    public class EmailFormViewModel
    {
        [Required(ErrorMessage = "Select Subject")]
        [Display(Name = "Select Subject *")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Enter Message")]
        [Display(Name = "Message Body *",Prompt = "Type message here....")]
        public string BodyMessage { get; set; }

        [ValidateNever]
        public IFormFile? SingleFile { get; set; }
        [ValidateNever]
        public List<IFormFile>? MultipleFile { get; set; }
    }
}
