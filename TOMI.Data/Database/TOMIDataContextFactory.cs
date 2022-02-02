using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TOMI.Data.Database
{
    public class TOMIDataContextFactory : IDesignTimeDbContextFactory<TOMIDataContext>
    {
        public TOMIDataContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TOMIDataContext>();
            builder.UseSqlServer("Server = (local); database = TomiDB; Integrated Security = true;");
            return new TOMIDataContext(builder.Options);
        }
    }
}
