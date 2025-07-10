using System.ComponentModel.DataAnnotations;

namespace AllDemo.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "UserId is Required")]
        [Display(Name = "User Id",Prompt = "Enter UserId")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [Display(Name = "Password", Prompt = "Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
       
    }
}
