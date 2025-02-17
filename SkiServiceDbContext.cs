using Microsoft.EntityFrameworkCore;
using SkiServiceAPI.Models;

namespace SkiServiceAPI
{
    public class SkiServiceDbContext : DbContext
    {
        public SkiServiceDbContext(DbContextOptions<SkiServiceDbContext> options) : base(options)
        {
        }

        // DbSets für die Entities
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
           .IsUnique();


            base.OnModelCreating(modelBuilder);

            // Configure entity properties and relationships if needed
            modelBuilder.Entity<Order>().Property(o => o.Service).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Order>().Property(o => o.Status).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.Email).IsRequired().HasMaxLength(100);

            modelBuilder.Entity<User>().Property(u => u.Username).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired();

            // Seed data
            modelBuilder.Entity<Order>().HasData(
                new Order { Id = 1, Service = "Kleiner Service", Status = "Offen", Email = "customer1@example.com", Name = "Customer 1", Phone = "+1234567890", Priority = "Standard", PickupDate = "2025-01-10", Comment= "test" },
                new Order { Id = 2, Service = "Grosser Service", Status = "InArbeit", Email = "customer2@example.com", Name = "Customer 2", Phone = "+9876543210", Priority = "Express", PickupDate = "2025-01-12"}
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "$2b$10$eIm3ck7feyg/tB4LBUSyaOQ198PPKs/cETcg4DdOE8aWsp4QVuls6", Role = "Admin" }, //Pwd:admin123
                new User { Id = 2, Username = "employee1", Password = "$2b$10$NoC03rw5UcCCOhG2peeHdOywOgiJ7SGhtxshVw88ME5A5dg0FiQxa", Role = "Employee" }, //Pwd: employee123
                new User { Id = 3, Username = "User1", Password = "$2b$10$I//na1FtDHJtyWkdyZyu9.kEqOsMUC.k0i46tZ8Vbog9zYyt3eg0O", Role = "User" } //Pwd: securepass123!
            );
        }

    }
}
