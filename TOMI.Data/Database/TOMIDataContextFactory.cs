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
            //   builder.UseSqlServer("Server=19-35059; database=TomiDB; User Id = sa; password = accuserv22;");
            //   builder.UseSqlServer("Server=19-35058; database=TomiDB; User Id = sa; password = accuserv22;");
            return new TOMIDataContext(builder.Options);
        }
    }
}
