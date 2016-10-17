using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.ViewModel
{
    public class AddStudyAbroadViewModel
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Semester")]
        public int Semester { get; set; }

        [Required]
        [Range(1900, 3000)]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Departure Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Credit Bearing")]
        public bool CreditBearing { get; set; }

        [Required]
        [Display(Name = "Internship")]
        public bool Internship { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int CountryId { get; set; }

        [StringLength(64)]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Program")]
        public int ProgramId { get; set; }

        [Display(Name = "Program Types")]
        public IEnumerable<int> ProgramTypes { get; set; }
    }

    public class StudyAbroadViewModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public StudentModel Student { get; set; }
        public int Semester { get; set; }
        public int Year { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool CreditBearing { get; set; }
        public bool Internship { get; set; }
        public int CountryId { get; set; }
        public string City { get; set; }
        public int ProgramId { get; set; }
        public IEnumerable<int> ProgramTypes { get; set; }
    }

    public class EditStudyAbroadViewModel : AddStudyAbroadViewModel
    {
        [Required]
        public int Id { get; set; }

        public static explicit operator EditStudyAbroadViewModel(StudyAbroadViewModel a)
        {
            return new EditStudyAbroadViewModel()
            {
                Id = a.Id,
                StudentId = a.StudentId,
                Semester = a.Semester,
                Year = a.Year,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                CreditBearing = a.CreditBearing,
                Internship = a.Internship,
                CountryId = a.CountryId,
                City = a.City,
                ProgramId = a.ProgramId,
                ProgramTypes = a.ProgramTypes
            };
        }
    }
}
