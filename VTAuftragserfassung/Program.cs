using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Reflection;
using System.Resources;
using VTAuftragserfassung.Controllers.Home;
using VTAuftragserfassung.Controllers.Login;
using VTAuftragserfassung.Database.Connection;
using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Database.DataAccess.Services;
using VTAuftragserfassung.Database.Repository;

namespace VTAuftragserfassung
{
    public class Program
    {
        #region Public Methods

        public static void Main(string[] args)
        {
            #region Services

            var builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

            // Add services to the container.
            builder.Services.AddSingleton<ISqlConnector>(conn => new SqlConnector(connectionString));
            builder.Services.AddSingleton<ResourceManager>(resM => new("VTAuftragserfassung.Database.DataAccess.MSSqlQueries", Assembly.GetExecutingAssembly()));
            builder.Services.AddScoped<ICachingService, CachingService>();
            builder.Services.AddScoped<IDataAccessService, DataAccessService>();
            builder.Services.AddScoped<IDataAccess<IDatabaseObject>, DatabaseAccess>();
            builder.Services.AddScoped<IDbRepository, DbRepository>();
            builder.Services.AddScoped<ILoginLogic, LoginLogic>();
            builder.Services.AddScoped<IHomeLogic, HomeLogic>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorOptions(
                options =>
                {
                    options.ViewLocationFormats.Add("/Views/Shared/Partials/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Shared/Partials/Details/{0}.cshtml");
                    options.ViewLocationFormats.Add("/Views/Shared/Partials/Forms/{0}.cshtml");
                });
            builder.Services.AddMemoryCache();
            builder.Services.AddResponseCompression();
            builder.Services.AddHttpContextAccessor();

            builder.Services.Configure<GzipCompressionProviderOptions>(
                options =>
                {
                    options.Level = CompressionLevel.Fastest;
                });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options =>
                {
                    options.LoginPath = new PathString("/Login");
                    options.LogoutPath = new PathString("/Login");
                    options.AccessDeniedPath = new PathString("/Login");
                });

            #endregion Builder

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapDefaultControllerRoute();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }

        #endregion Public Methods
    }
}