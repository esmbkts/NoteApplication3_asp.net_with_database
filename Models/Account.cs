using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp.net.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public int IsActive { get; set; }

        public User User { get; set; } // Account'un bir User'ı var, Navigation property
        public ICollection<Note> Notes { get; set; } // Account'un birden fazla Note'u olabilir

        // Kullanıcının e-posta ve şifresini kontrol eden metod
        public bool CheckCredentials(string username, string password)
        {
            return Username == username && Password == password;
        }
    }
}
