using GutenbergProject.Entities;
using Microsoft.EntityFrameworkCore;

namespace GutenbergProject
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserBook> UserBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.userName)
                .IsUnique();

            modelBuilder.Entity<UserBook>()
                .HasOne<User>(ub => ub.User)
                .WithMany(u => u.UserBooks)
                .HasForeignKey(ub => ub.userId);

            modelBuilder.Entity<UserBook>()
                .HasIndex(ub => ub.bookId)
                .IsUnique();

        }
    }
}
