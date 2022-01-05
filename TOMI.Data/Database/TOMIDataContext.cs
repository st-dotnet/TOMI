using Microsoft.EntityFrameworkCore;
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
        }
    }
}
