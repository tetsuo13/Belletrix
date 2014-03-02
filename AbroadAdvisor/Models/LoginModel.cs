using System.ComponentModel.DataAnnotations;

namespace Bennett.AbroadAdvisor.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        [StringLength(24)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(256)]
        public string Password { get; set; }
    }
}
