using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using System;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using TOMI.Data.Database;
using TOMI.Services.Common.Extensions;
using TOMI.Services.Helpers;
using TOMI.Services.Interfaces;
using TOMI.Services.Interfaces.CustomerService;
using TOMI.Services.Interfaces.RangesService;
using TOMI.Services.Repository;

namespace TOMI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
        }
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _webHostEnvironment;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContextPool<TOMIDataContext>(options => options.UseSqlServer(connectionString));

            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("foo",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            services.AddControllers().AddNewtonsoftJson(options =>
              options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );


            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });


            IMapper mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);
            //  services.AddTomiServiceExtension(Configuration.GetConnectionString("DefaultConnection"));
            services.AddTransient<ICustomerService, CustomerRepository>();
            services.AddTransient<IUserService, UserRepository>();
            services.AddTransient<IStoreService, StoreRepository>();
            services.AddTransient<IRangesService, RangeRepository>();
            services.AddTransient<IGroupService, GroupRepository>();
            services.AddTransient<IStockAdjustmentService, StockAdjustmentRepository>();
            services.AddTransient<IInfoLoadService, InfoLoadRepository>();
            services.AddTransient<IProgramTerminalService, ProgramTerminalRepository>();
           // services.AddTransient<IFileStoreService, FileStoreRepository>();
            services.AddTransient<IReportOptionService, ReportOptionRepository>();
            services.AddTransient<IInfomrationLoadingService,InfomrationLoadingService>();

            // add Swagger 
            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {

                        Title = "TOMI",
                        Version = "v1"
                    });
                setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TOMI v1"));

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(_webHostEnvironment.WebRootPath, "Upload")),
                RequestPath = "/files"
            });

            app.UseHttpsRedirection();

            app.UseExceptionHandler("/error"); // Add this
            //app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseRouting();
            app.UseCors("foo"); // second
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseMiddleware<AuthMiddleware>();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
