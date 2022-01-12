using System.ComponentModel.DataAnnotations;

namespace Zenzinger_Eshop_Web.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Phone]
        public string Phone { get; set; }
        
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{4,}$", ErrorMessage = RegisterViewModel.ErrorMessagePassword)]
        public string Password { get; set; }
        private const string ErrorMessagePassword = "Passwords must be at least 4 characters.<BR>Passwords must have at least one non alphanumeric character.<BR>Passwords must have at least one digit('0'-'9').<BR>Passwords must have at least one uppercase('A'-'Z').";

        
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match!")]
        public string RepeatedPassword { get; set; }
    }
}