using System;
using System.ComponentModel.DataAnnotations;

namespace Belletrix.Entity.Model
{
    public class UserModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(64)]
        public string LastName { get; set; }

        [Required]
        [StringLength(24)]
        [Editable(false)]
        public string Login { get; set; }

        public int PasswordIterations { get; set; }
        public string PasswordSalt { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Display(Name = "Last Login")]
        public DateTime LastLogin { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(128)]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Administrator?")]
        public bool IsAdmin { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
