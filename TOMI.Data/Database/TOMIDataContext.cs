using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Data.Database
{
    public class TOMIDataContext : DataContextBase<TOMIDataContext>
    {
        public object test;

        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
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
        public DbSet<Terminal_Smf> Terminal_Smf { get; set; }
        public DbSet<Terminal_Department> Terminal_Department { get; set; }
        public DbSet<FileStore> FileStore { get; set; }
        public DbSet<UploadFileName> UploadFileName { get; set; }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<StockAdjustmentlog> StockAdjustmentlog { get; set; }
 
        public DbSet<getInventoryFigureData> getInventoryFigureData { get; set; }
        public DbSet<getInventario> getInventarios { get; set; }
        public DbSet<getMarbete> getMarbetes { get; set; }
        public DbSet<spCodeNotfoundReport> spCodeNotfoundReport { get; set; }
        public DbSet<Ttransmission_Summary> Ttransmission_Summary { get; set; }
        public DbSet<spInformationLoading> spInformationLoading { get; set; }
        public DbSet<spgetVoidTagData> spgetVoidTagData { get; set; }

        public DbSet<spInformationTransmissionDetails> spInformationTransmissionDetails { get; set; }

        public DbSet<spTerminalForOriginalDetials> spTerminalForOriginalDetials { get; set; }
        public DbSet<spOriginalTag> spOriginalTag { get; set; }
        public DbSet<MTerminalSummary> MTerminalSummary { get; set; }

        public DbSet<BillingReport> billingReports { get; set; }
        public DbSet<spGenerateGenerateMF1> spGenerateGenerateMF1 { get; set; }
        public TOMIDataContext(DbContextOptions<TOMIDataContext> options)
            : base(options)
        {
            Database.SetCommandTimeout(150000);
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
            modelBuilder.Entity<Terminal_Department>()
             .HasMany(c => c.MF1)
             .WithOne(c => c.MF2)
             .HasForeignKey(x => x.Department);
            //modelBuilder.Entity<OrderJob>()
            //.HasMany(c => c.MF1)
            //.WithOne(c => c.OrderJob)
            //.HasForeignKey(x => x.CustomerId);



            modelBuilder.Entity<Store>()
            .HasMany(c=> c.MF1)
            .WithOne(c => c.Store)
            .HasForeignKey(x => x.StoreId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Customer>()
           .HasMany(c => c.MF1)
           .WithOne(c => c.customer)
           .HasForeignKey(x => x.CustomerId)
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
                EmployeeNumber="987654",
            };

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
          
            user.Password = passwordHasher.HashPassword(user, "Sss1234!");
            builder.Entity<User>().HasData(user);
        }

       
    }
}
