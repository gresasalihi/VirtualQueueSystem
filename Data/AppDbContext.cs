using Microsoft.EntityFrameworkCore;
using VirtualQueueSystem.Models;

namespace VirtualQueueSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
            public DbSet<Feedback> Feedbacks { get; set; }

        public DbSet<Order> Orders { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=VirtualQueueSystem.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ✅ Define Reservation -> ReservationItem Relationship
       
modelBuilder.Entity<Feedback>()
    .HasOne(f => f.User)
    .WithMany()
    .HasForeignKey(f => f.UserId)
    .OnDelete(DeleteBehavior.Cascade);

            // ✅ Order & User Relationship
            modelBuilder.Entity<Order>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Order: Ensure `QueueNumber` is unique and required
            modelBuilder.Entity<Order>()
                .Property(o => o.QueueNumber)
                .IsRequired();

            // ✅ Order: Ensure `Status` is required
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .IsRequired();

            // ✅ Seed Initial Menu Data
            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem { Id = 1, Name = "Pancakes", Category = "Breakfast", Price = 5.99M, ImageUrl = "/images/pancakes.jpg" },
                new MenuItem { Id = 2, Name = "Omelette", Category = "Breakfast", Price = 4.50M, ImageUrl = "/images/omelette.jpg" },
                new MenuItem { Id = 3, Name = "Grilled Chicken", Category = "Lunch", Price = 9.99M, ImageUrl = "/images/grilled-chicken.jpg" },
                new MenuItem { Id = 4, Name = "Caesar Salad", Category = "Lunch", Price = 6.50M, ImageUrl = "/images/caesar-salad.jpg" },
                new MenuItem { Id = 5, Name = "Steak", Category = "Dinner", Price = 14.99M, ImageUrl = "/images/steak.jpg" },
                new MenuItem { Id = 6, Name = "Salmon", Category = "Dinner", Price = 12.99M, ImageUrl = "/images/salmon.jpg" },
                new MenuItem { Id = 7, Name = "French Fries", Category = "Snacks", Price = 3.99M, ImageUrl = "/images/fries.jpg" },
                new MenuItem { Id = 8, Name = "Chicken Nuggets", Category = "Snacks", Price = 4.99M, ImageUrl = "/images/nuggets.jpg" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
