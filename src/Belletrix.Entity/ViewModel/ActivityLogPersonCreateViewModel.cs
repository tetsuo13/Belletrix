using System;
using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.ViewModel
{
    public class ActivityLogPersonCreateViewModel
    {
        [Required]
        [Display(Name = "Name")]
        [MaxLength(128)]
        public string FullName { get; set; }

        public Guid SessionId { get; set; }

        [MaxLength(256)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Phone Number")]
        [MaxLength(32)]
        [Phone]
        public string PhoneNumber { get; set; }

        [MaxLength(128)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please select a type")]
        public int Type { get; set; }
    }
}
