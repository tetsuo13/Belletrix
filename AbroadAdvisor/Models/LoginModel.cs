using System.ComponentModel.DataAnnotations;

namespace Bennett.AbroadAdvisor.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter your user name")]
        [Display(Name = "Username")]
        [StringLength(24)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "A password is required to continue")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(256)]
        public string Password { get; set; }
    }
}
