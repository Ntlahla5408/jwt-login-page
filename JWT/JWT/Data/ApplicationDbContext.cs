using JWT.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWT.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(user => user.Username).HasMaxLength(50).IsRequired();
                entity.Property(user => user.Email).HasMaxLength(254).IsRequired();
                entity.Property(user => user.Password).HasMaxLength(100).IsRequired();
                entity.Property(user => user.Role).HasMaxLength(20).IsRequired();

                entity.HasIndex(user => user.Username).IsUnique();
                entity.HasIndex(user => user.Email).IsUnique();
            });
        }
    }
}
