using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Belletrix.Entity.ViewModel
{
    public class ActivityLogCreateViewModel
    {
        [Required]
        [Display(Name = "Main")]
        [StringLength(256)]
        public string Title { get; set; }

        [Display(Name = "Subtitle")]
        [StringLength(256)]
        public string Title2 { get; set; }

        [Display(Name = "Additional")]
        [StringLength(256)]
        public string Title3 { get; set; }

        [StringLength(256)]
        public string Organizers { get; set; }

        [StringLength(512)]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Starting Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Ending Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "On Campus?")]
        public bool? OnCampus { get; set; }

        [StringLength(2048)]
        [Display(Name = "Web Site")]
        [Url]
        public string WebSite { get; set; }

        [StringLength(4096)]
        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Required(ErrorMessage = "Please select at least one type")]
        public IEnumerable<int> Types { get; set; }

        public Guid SessionId { get; set; }

        public static explicit operator ActivityLogCreateViewModel(ActivityLogModel a)
        {
            return new ActivityLogCreateViewModel()
            {
                Title = a.Title,
                Title2 = a.Title2,
                Title3 = a.Title3,
                Organizers = a.Organizers,
                Location = a.Location,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                OnCampus = a.OnCampus,
                WebSite = a.WebSite,
                Notes = a.Notes
            };
        }
    }

    public class ActivityLogEditViewModel : ActivityLogCreateViewModel
    {
        [Required]
        public int Id { get; set; }

        public static explicit operator ActivityLogEditViewModel(ActivityLogModel a)
        {
            return new ActivityLogEditViewModel()
            {
                Id = a.Id,
                Title = a.Title,
                Title2 = a.Title2,
                Title3 = a.Title3,
                Organizers = a.Organizers,
                Location = a.Location,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                OnCampus = a.OnCampus,
                WebSite = a.WebSite,
                Notes = a.Notes,
                Types = a.Types.Cast<int>()
            };
        }
    }

    public class ActivityLogPersonCreateViewModel
    {
        [Required]
        [Display(Name = "Name")]
        [StringLength(128)]
        public string FullName { get; set; }

        public Guid SessionId { get; set; }

        [StringLength(256)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(32)]
        [Phone]
        public string PhoneNumber { get; set; }

        [StringLength(128)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select a type")]
        public int Type { get; set; }
    }

    public class ActivityLogViewViewModel
    {
        public ActivityLogModel ActivityLog { get; set; }

        public IEnumerable<ActivityLogParticipantModel> Participants { get; set; }

        public UserModel CreatedBy { get; set; }
    }
}
