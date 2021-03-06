using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
namespace TOMI
{
    public class Program
    {
        public static void Main(string[] args)
        {

            //Log.Logger = new LoggerConfiguration()
            //   .MinimumLevel.Debug()
            //   .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            //   .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            //   .Enrich.FromLogContext()
            //   .WriteTo.File("Logs/logs.txt", rollingInterval: RollingInterval.Day)
            //   .CreateLogger();
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
