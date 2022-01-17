using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TOMI.Data.Database.Entities;

namespace TOMI.Data.Database
{
    public class DataContextBase<TContext> : DbContext where TContext : DbContext
    {
        public DataContextBase(DbContextOptions<TContext> options)
               : base(options)
        {
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // update entity timestamps
            var entities = ChangeTracker.Entries().Where(x => x.Entity is EntityBase && (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));
            var timestamp = DateTimeOffset.Now;

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((EntityBase)entity.Entity).CreatedAt = timestamp;
                }
                else if (entity.State == EntityState.Modified)
                {
                    ((EntityBase)entity.Entity).UpdatedAt = timestamp;
                }
                else if (entity.State == EntityState.Deleted)
                {
                    ((EntityBase)entity.Entity).DeletedAt = timestamp;
                    entity.State = EntityState.Modified;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
