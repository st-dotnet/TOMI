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
        public DbSet<Sales> Sales { get; set; }
        public DbSet<Stocks> Stocks { get; set; }
        public DbSet<Master> Master { get; set; }
        public DbSet<Ranges> Ranges { get; set; }
        public DbSet<Group> Group { get; set; }
        public DbSet<StockAdjustment> StockAdjustment { get; set; }

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


            modelBuilder.Entity<Group>()
                  .HasMany(c => c.Ranges)
                  .WithOne(c=>c.Group)
                   .HasForeignKey(x => x.GroupId);

            modelBuilder.Entity<Master>()
              .HasMany(c => c.StockAdjustment)
              .WithOne(c => c.Master)
               .HasForeignKey(x => x.SKU);

            modelBuilder.Entity<Store>()
                .HasOne(c => c.User)
                .WithOne(s => s.Store)
                .HasForeignKey<User>(x => x.StoreId)
                .OnDelete(DeleteBehavior.NoAction);

            this.SeedUsers(modelBuilder);
        }
        private void SeedUsers(ModelBuilder builder)
        {
            // add first  organization for user customer id 
            Customer customer = new Customer()
            {
                Id = Guid.Parse("b74ddd14-6340-4840-95c2-db12554843e5"),
                Name = "Test"
            };
            builder.Entity<Customer>().HasData(customer);

            // add  first User in Database User table
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
          
            user.Password = passwordHasher.HashPassword(user, "Sss1234!");
            builder.Entity<User>().HasData(user);
        }
    }
}
