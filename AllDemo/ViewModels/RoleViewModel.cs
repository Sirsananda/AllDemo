using System.ComponentModel.DataAnnotations;
using AllDemo.Models;

namespace AllDemo.ViewModels
{
    public class RoleViewModel
    {
        [Required(ErrorMessage = "Role Name is Required")]
        [Display(Name = "Role Name", Prompt = "Enter Role Name")]
        public string RoleName { get; set; }

        public List<RoleModel>? RoleList { get; set; }
    }
}
