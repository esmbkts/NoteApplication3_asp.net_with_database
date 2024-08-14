using asp.net.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp.net.Models
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [ForeignKey("Account")]  //
        [Required]
        public int AccountId { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public Account Account { get; set; } // Note'un bir Account'u var,  Navigation property         

        // Parametresiz kurucu metod (EF Core için gereklidir)
        public Note() { }

        // Parametreli kurucu metod
        public Note(string description, DateTime createTime)
        {
            Description = description;
            CreateTime = createTime;
            LastUpdateTime = DateTime.Now;
        }
    }
}