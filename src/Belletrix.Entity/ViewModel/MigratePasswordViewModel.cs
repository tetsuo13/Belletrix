using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.ViewModel
{
    public class MigratePasswordViewModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter your current password")]
        [StringLength(256)]
        public string CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please enter a new password")]
        [StringLength(256)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please reenter your new password")]
        [StringLength(256)]
        [Compare("NewPassword", ErrorMessage = "Passwords don't match")]
        public string NewPasswordConfirm { get; set; }
    }
}
