using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllDemo.Models
{
    [Table("tblRole")]
    public class RoleModel
    {
        [Key]
        public int? RollId { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string RoleName { get; set; }
    }
}
