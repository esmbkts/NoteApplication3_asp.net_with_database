using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp.net.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int IsActive { get; set; }

        [Required]
        public int IsAdmin { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public ICollection<Account> Accounts { get; set; } // User'ın birden fazla Account'u olabilir

        public User(){}

        public User(string email, int isAdmin, int isActive, string firstname = null, string surname = null, string phone = null)
        {
            Firstname = firstname;
            Surname = surname;
            Email = email;
            IsAdmin = isAdmin;
            IsActive = 1;
            CreateTime = DateTime.Now;
            LastUpdateTime = DateTime.Now;
        }
    }
}
