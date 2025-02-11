using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel()
        {
            
        }
        public SignUpViewModel(string userName, string phone, string email, string password, string confirmPassword)
        {
            UserName = userName;
            Phone = phone;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }
        [Required(ErrorMessage = "UserName field cannot be left blank !")]
        [Display(Name = "User Name : ")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Phone number field cannot be left blank !")]
        [RegularExpression(@"^[1-9]{1}[0-9]{9}$", ErrorMessage = "Phone number must be 10 digits and cannot start with 0.")]
        [Display(Name = "Phone Number : ")]
        public string Phone { get; set; }
        [EmailAddress(ErrorMessage = "Wrong email format!")]
        [Required(ErrorMessage = "Email field cannot be left blank !")]
        [Display(Name = "Email Address : ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password field cannot be left blank !")]
        [Display(Name = "Password : ")]
        public string Password { get; set; }
        [Compare(nameof(Password),ErrorMessage = "Passwords are not the same.")]
        [Required(ErrorMessage = "Password Confirm field cannot be left blank !")]
        [Display(Name = "Confirm Password : ")]
        public string ConfirmPassword { get; set; }
    }
}
