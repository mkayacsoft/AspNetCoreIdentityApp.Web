using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignInViewModel
    {
        public SignInViewModel()
        {
            
        }
        public SignInViewModel(string email, string password, bool rememberMe)
        {
            Email = email;
            Password = password;
            RememberMe = rememberMe;
            
        }

        [EmailAddress(ErrorMessage = "Wrong email format!")]
        [Required(ErrorMessage = "Email field cannot be left blank !")]
        [Display(Name = "Email Address : ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password field cannot be left blank !")]
        [Display(Name = "Password : ")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
