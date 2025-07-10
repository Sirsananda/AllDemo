using System.ComponentModel.DataAnnotations;
using AllDemo.Models;

namespace AllDemo.ViewModels
{
    public class RegisterViewModel
    {
        public int? UserId { get; set; }

        [Display(Name="User Name",Prompt = "Enter User Name")]
        [Required(ErrorMessage="UserName Required")]
        public string UserName { get; set; }

        [Display(Name = "User Email", Prompt = "Enter User Email")]
        [Required(ErrorMessage = "UserEmail Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Enter a valid email address")]
        public string UserEmail { get; set; }

        [Display(Name = "Password",Prompt = "Enter Password")]
        [Required(ErrorMessage = "Password Required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password",Prompt = "Re Enter Password")]
        [Required(ErrorMessage = "Confirm Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Role List")]
        [Required(ErrorMessage = "Select a Role")]
        public int? RoleId { get; set; }
       
        public List<RoleModel> Roles { get; set; } = new();
    }
}
