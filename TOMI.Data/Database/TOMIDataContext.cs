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
        public DbSet<InfoLoad> InfoLoad { get; set; }
        public DbSet<DwnErrors> DwnErrors { get; set; }
        public DbSet<OrderJob> OrderJob { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<Reserved> Reserved { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<ParametersByDepartment> ParametersByDepartment { get; set; }
        public DbSet<MF1> MF1 { get; set; }
        public DbSet<MF2> MF2 { get; set; }

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

            modelBuilder.Entity<OrderJob>()
         .HasMany(c => c.StockAdjustment)
         .WithOne(c => c.OrderJob)
          .HasForeignKey(x => x.SKU);

            modelBuilder.Entity<Store>()
 .HasOne(c => c.User)
 .WithOne(s => s.Store)
 .HasForeignKey<User>(x => x.StoreId)
 .OnDelete(DeleteBehavior.NoAction);




            //modelBuilder.Entity<MF2>()
            // .HasMany(c => c.MF1)
            // .WithOne(c => c.MF2)
            // .HasForeignKey(x => x.Department);




            modelBuilder.Entity<OrderJob>()
            .HasMany(c => c.MF1)
            .WithOne(c => c.OrderJob)
            .HasForeignKey(x => x.CustomerId);



            modelBuilder.Entity<Store>()
            .HasMany(c => c.MF1)
            .WithOne(c => c.Store)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.NoAction);
            //modelBuilder.Entity<OrderJob>()
            // .HasMany(c => c.MF1)
            // .WithOne(c => c.OrderJob)
            // .HasForeignKey(x => x.Inventory_Date);



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
