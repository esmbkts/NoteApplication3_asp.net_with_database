using System.ComponentModel.DataAnnotations;

namespace asp.net.Models
{
    public class UpdateUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; } // Şifre isteğe bağlı olabilir
    }
}
