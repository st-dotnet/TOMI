using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TOMI.Data.Database
{
    public class TOMIDataContextFactory : IDesignTimeDbContextFactory<TOMIDataContext>
    {
        public TOMIDataContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TOMIDataContext>();
            builder.UseSqlServer("Server=localhost;Database=TOMI;Integrated Security=True");
            return new TOMIDataContext(builder.Options);
        }
    }
}
