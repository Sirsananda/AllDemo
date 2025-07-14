using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllDemo.Models
{
    [Table("tblRole")]
    public class RoleModel
    {
        [Key]
        public int? RollId { get; set; }

        [Required(ErrorMessage = "Role Name is Required")]
        [Display(Name="Role Name",Prompt = "Enter Role Name")]
        public string RoleName { get; set; }
    }
}
