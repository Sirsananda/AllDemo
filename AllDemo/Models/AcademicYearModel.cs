using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllDemo.Models
{
    [Table("tblAcademicYear")]
    public class AcademicYearModel
    {
        [Key]
        public int Year_Id { get; set; }

        [Required(ErrorMessage = "Enter Academic Year")]
        [MaxLength(20)]
        [Display(Name = "Year Name",Prompt = "YYYY-YYYY")]
        public string Year_Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choose Start Date")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateOnly? Start_Date { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Choose End Date")]
        [Display(Name = "End Date")]
        public DateOnly? End_Date { get; set; }
    }
}
