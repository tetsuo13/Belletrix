using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.Model
{
    public class StudyAbroadModel
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        public StudentModel Student { get; set; }

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
}
