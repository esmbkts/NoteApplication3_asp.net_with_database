using Microsoft.EntityFrameworkCore;
using asp.net.Models;
namespace asp.net.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Var olan tabloları belirtmek için:
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users"); // Mevcut tablonun adı
                entity.HasKey(e => e.Id);

                // Var olan tablo sütunlarını eşleştirme
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Firstname).HasColumnName("Firstname");
                entity.Property(e => e.Surname).HasColumnName("Surname");
                entity.Property(e => e.Email).HasColumnName("MailAddress");
                entity.Property(e => e.IsActive).HasColumnName("IsActive");
                entity.Property(e => e.IsAdmin).HasColumnName("IsAdmin");
                entity.Property(e => e.CreateTime).HasColumnName("CreateTime");
                entity.Property(e => e.LastUpdateTime).HasColumnName("LastUpdateTime");
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Accounts"); // Mevcut tablonun adı
                entity.HasKey(e => e.Id);
                entity.HasOne(a => a.User) // User ile ilişki
               .WithMany(u => u.Accounts) // User'ın birden fazla Account'u olabilir
               .HasForeignKey(a => a.UserId); // Foreign Key

                // Var olan tablo sütunlarını eşleştirme
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId");
                entity.Property(e => e.Username).HasColumnName("Username");
                entity.Property(e => e.Password).HasColumnName("Password");
                entity.Property(e => e.IsActive).HasColumnName("IsActive");
            });

            modelBuilder.Entity<Note>(entity =>
            {                entity.ToTable("Notes"); // Mevcut tablonun adı
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(n => n.Account) // Account ile ilişki
                .WithMany(a => a.Notes) // Account'un birden fazla Note'u olabilir
                .HasForeignKey(n => n.AccountId); // Foreign Key

                // Var olan tablo sütunlarını eşleştirme
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.AccountId).HasColumnName("AccountId");
                entity.Property(e => e.CreateTime).HasColumnName("CreateTime");
                entity.Property(e => e.LastUpdateTime).HasColumnName("LastUpdateTime");
            });
        }

    }
}
        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationships and configurations if needed

            // User-Account Relationship
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);

            // Note-Account Relationship
            modelBuilder.Entity<Note>()
                .HasOne(n => n.Account)
                .WithMany()
                .HasForeignKey(n => n.AccountId);
        }*/


/*
 
 public class User
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int IsActive { get; set; }
        public int IsAdmin { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }

    public class Note
    {
        public int id { get; set; }
        public string description { get; set; }
        public int AccountId { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }

    public class Account
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int IsActive { get; set; }
    }
 
 */