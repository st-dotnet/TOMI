using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using TOMI.Data.Database.Entities;

namespace TOMI.Data.Database
{
    public class TOMIDataContext : DataContextBase<TOMIDataContext>
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }

        public TOMIDataContext(DbContextOptions<TOMIDataContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Stores)
                .WithOne(s => s.Customer)
                .HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Users)
                .WithOne(s => s.Customer)
                .HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<User>()
                .HasOne(c => c.Store)
                .WithOne(s => s.User)
                .HasForeignKey<Store>(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Store>()
                .HasOne(c => c.User)
                .WithOne(s => s.Store)
                .HasForeignKey<User>(x => x.StoreId)
                .OnDelete(DeleteBehavior.NoAction);

            this.SeedUsers(modelBuilder);
        }
        private void SeedUsers(ModelBuilder builder)
        {
            Customer customer = new Customer()
            {
                Id = Guid.Parse("b74ddd14-6340-4840-95c2-db12554843e5"),
                Name = "Test"
            };
            builder.Entity<Customer>().HasData(customer);
            User user = new User()
            {
                Id = Guid.Parse("b74ddd14-6340-4840-95c2-db12554843e5"),
                FirstName = "Admin",
                LastName="Admin",
                Email = "admin@gmail.com",
                Role = RoleType.SuperAdmin.ToString(),
                PhoneNumber = "1234567890",
                CustomerId = Guid.Parse("b74ddd14-6340-4840-95c2-db12554843e5"),
            };

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            passwordHasher.HashPassword(user, "Sss1234!");

            builder.Entity<User>().HasData(user);
        }
    }
}
