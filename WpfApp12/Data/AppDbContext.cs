using Microsoft.EntityFrameworkCore;
using WpfApp12.Models;

namespace WpfApp12.Data
{
    /// <summary>Контекст EF Core: таблица Books и настройки подключения.</summary>
    public class AppDbContext : DbContext
    {
        private readonly string _connectionString;

        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Books");
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Author).IsRequired().HasMaxLength(200);
                entity.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

                entity.HasData(new Book
                {
                    Id = 1,
                    Title = "Война и мир",
                    Author = "Л.Н. Толстой",
                    Year = 1869,
                    Status = BookStatus.Available
                });
            });
        }
    }
}
