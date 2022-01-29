using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TOMI.Data.Database
{
    public class TOMIDataContextFactory : IDesignTimeDbContextFactory<TOMIDataContext>
    {
        public TOMIDataContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TOMIDataContext>();
            builder.UseSqlServer("Server=LAPTOP-115NHTT9\\SQLEXPRESS;database=TomiDB;User ID= sa;password=123456;");
            return new TOMIDataContext(builder.Options);
        }
    }
}
