using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AllDemo.Models
{
    [Table("tblAdmissionType")]
    public class AdmissionTypeModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]// Auto-increment (Identity in SQL Server)
        public int AdmissionTypeId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Admission Type", Prompt = "Enter Admission Type")]
        public string AdmissionTypeName { get; set; }

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; } = false;
    }
}
