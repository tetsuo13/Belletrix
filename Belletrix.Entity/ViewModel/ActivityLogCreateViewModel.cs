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
        public string StartDate { get; set; }

        [Required]
        [Display(Name = "Ending Date")]
        [DataType(DataType.Date)]
        public string EndDate { get; set; }

        [Required]
        [Display(Name = "On Campus?")]
        public bool OnCampus { get; set; }

        [MaxLength(2048)]
        [DataType(DataType.Url)]
        public string WebSite { get; set; }

        [MaxLength(4096)]
        public string Notes { get; set; }
    }
}
