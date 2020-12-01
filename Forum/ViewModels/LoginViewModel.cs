using System.ComponentModel.DataAnnotations;

namespace Forum.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Empty login")]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Empty password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
