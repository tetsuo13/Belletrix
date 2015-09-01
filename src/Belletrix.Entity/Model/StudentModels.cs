using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belletrix.Entity.Model
{
    public interface IStudentModel
    {
    }

    /// <summary>
    /// Student model related to student entry by staff.
    /// </summary>
    public class StudentModel : StudentBaseModel, IStudentModel
    {
        [Display(Name = "Phi Beta Delta?")]
        public bool? PhiBetaDeltaMember { get; set; }

        public int NumberOfNotes { get; set; }

        /// <summary>
        /// Set of promos that the student may be associated with.
        /// </summary>
        [Display(Name = "Promo")]
        public IEnumerable<int> PromoIds { get; set; }
    }

    /// <summary>
    /// Standard student model.
    /// </summary>
    public class StudentBaseModel
    {
        public int Id { get; set; }

        public DateTime Created { get; set; }

        [Display(Name = "Initial Meeting")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? InitialMeeting { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(64)]
        public string FirstName { get; set; }

        [StringLength(64)]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(64)]
        public string LastName { get; set; }

        [Display(Name = "Living")]
        public bool? LivingOnCampus { get; set; }

        [StringLength(128)]
        [Display(Name = "Local Address")]
        public string StreetAddress { get; set; }

        [StringLength(128)]
        public string StreetAddress2 { get; set; }

        [StringLength(128)]
        public string City { get; set; }

        [StringLength(32)]
        public string State { get; set; }

        [StringLength(16)]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [StringLength(32)]
        [Display(Name = "Telephone #")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Range(1900, 3000)]
        [Display(Name = "Entering Year")]
        public int? EnteringYear { get; set; }

        [Range(1900, 3000)]
        [Display(Name = "Graduating Year")]
        public int? GraduatingYear { get; set; }

        [Range(0, 3)]
        [Display(Name = "Classification")]
        public int? Classification { get; set; }

        [StringLength(32)]
        [Display(Name = "Student ID")]
        public string StudentId { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Enrolled as a")]
        public bool? EnrolledFullTime { get; set; }

        public int? Citizenship { get; set; }

        [Display(Name = "Pell Grant?")]
        public bool? PellGrantRecipient { get; set; }

        [Display(Name = "Passport Holder?")]
        public bool? HasPassport { get; set; }

        [Range(0.00, 9.99)]
        [Display(Name = "Current GPA")]
        public double? Gpa { get; set; }

        [StringLength(128)]
        [EmailAddress]
        [Display(Name = "Bennett College Email")]
        [DataType(DataType.EmailAddress)]
        public string CampusEmail { get; set; }

        [StringLength(128)]
        [EmailAddress]
        [Display(Name = "Alternative Email")]
        [DataType(DataType.EmailAddress)]
        public string AlternateEmail { get; set; }

        [Display(Name = "Majors")]
        public IEnumerable<int> SelectedMajors { get; set; }

        [Display(Name = "Minors")]
        public IEnumerable<int> SelectedMinors { get; set; }

        [Display(Name = "Fluency")]
        public IEnumerable<int> SelectedLanguages { get; set; }

        [Display(Name = "Studied")]
        public IEnumerable<int> StudiedLanguages { get; set; }

        [Display(Name = "Country")]
        public IEnumerable<int> StudyAbroadCountry { get; set; }

        [Display(Name = "Year")]
        public IEnumerable<int> StudyAbroadYear { get; set; }

        /// <summary>
        /// Not used by promos.
        /// </summary>
        [Display(Name = "Desired Language Abroad")]
        public IEnumerable<int> SelectedDesiredLanguages { get; set; }

        /// <summary>
        /// Values will be indexes from
        /// StudentStudyAbroadWishlistModel.PeriodValue
        /// </summary>
        [Display(Name = "Semester")]
        public IEnumerable<int> StudyAbroadPeriod { get; set; }
    }
}
