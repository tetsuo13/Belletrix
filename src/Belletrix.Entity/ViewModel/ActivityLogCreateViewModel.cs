using Belletrix.Entity.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.ViewModel
{
    public class ActivityLogCreateViewModel
    {
        [Required]
        [Display(Name = "Title")]
        [MaxLength(256)]
        public string Title { get; set; }

        [MaxLength(256)]
        public string Title2 { get; set; }

        [MaxLength(256)]
        public string Title3 { get; set; }

        [MaxLength(256)]
        public string Organizers { get; set; }

        [MaxLength(512)]
        public string Location { get; set; }

        [Required]
        [Display(Name = "Starting Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Ending Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "On Campus?")]
        public bool OnCampus { get; set; }

        [MaxLength(2048)]
        [Display(Name = "Web Site")]
        [Url]
        public string WebSite { get; set; }

        [MaxLength(4096)]
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
}
