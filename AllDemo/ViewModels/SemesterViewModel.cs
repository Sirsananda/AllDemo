using AllDemo.Models;
using System.ComponentModel.DataAnnotations;

namespace AllDemo.ViewModels
{
    public class SemesterViewModel
    {
        public int? SemesterId { get; set; }

        [Required(ErrorMessage = "Select Academic Year !")]
        [Display(Name = "Academic Year")]
        public int? Academic_Year_id { get; set; }

        [Required(ErrorMessage = "Input Semester Name")]
        [Display(Name = "Semester Name", Prompt = "Input Semester Name")]
        public string SemesterName { get; set; }

        [Display(Name = "Choose Start Date")]
        [DataType(DataType.Date)]
        public DateOnly? StartDate { get; set; }

        [Display(Name = "Choose End Date")]
        [DataType(DataType.Date)]
        public DateOnly? EndDate { get; set; }

        public List<AcademicYearModel> AcademicYears { get; set; } = new();
    }
}
