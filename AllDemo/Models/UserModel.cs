using System.ComponentModel.DataAnnotations;//to call for predefined attributes
using System.ComponentModel.DataAnnotations.Schema;//to call the table attribute to give a user defined table name


namespace AllDemo.Models
{
    [Table("tblUsers")]
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        [Required(ErrorMessage="Enter Your Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="Enter Your Email")]
        public string UserEmail { get; set; }
        [Required(ErrorMessage ="Enter Password")]
        public string HashPassword { get; set; }
        public int? RoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}
