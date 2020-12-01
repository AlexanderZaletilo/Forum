using System.ComponentModel.DataAnnotations;

namespace Forum.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Empty login")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Empty email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Empty password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Repeat Password")]
        public string PasswordConfirm { get; set; }
    }
}
