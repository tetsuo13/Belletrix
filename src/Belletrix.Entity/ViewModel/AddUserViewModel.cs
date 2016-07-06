using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.ViewModel
{
    public class AddUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(64)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(64)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Login Username")]
        [StringLength(24)]
        [Editable(false)]
        public string Login { get; set; }

        // TODO: Rename this to "Password" and remove the obsolete properties.
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        [StringLength(128)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Administrator?")]
        public bool IsAdmin { get; set; }

        [Required]
        [Display(Name = "Active?")]
        public bool IsActive { get; set; }
    }
}
