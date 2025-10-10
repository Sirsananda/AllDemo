using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AllDemo.Models
{
    [Table("tblSemester")]
    public class SemesterModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//auto-incremented(identity in sql server)
        public int SemesterId { get; set; }

        #region here i use the FK of Academic year table of Year_Id
        [Required]
        [Display(Name ="Academic Year")]
        [ForeignKey("AcademicYear")] //This is the Foreign Key property
        public int Year_Id { get; set; }//Explicit Foreign Key of the tblAcademicYear table(model class AcademicYearModel)

        // This is the Navigation Property
        public AcademicYearModel AcademicYear { get; set; }
        #endregion

        public string SemesterName { get; set; } = string.Empty;
        
        public DateOnly? StartDate { get; set; }
        
        public DateOnly? EndDate { get; set; }

      
    }
}
