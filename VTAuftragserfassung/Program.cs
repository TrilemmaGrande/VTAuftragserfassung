using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Reflection;
using System.Resources;
using VTAuftragserfassung.Controllers.Home;
using VTAuftragserfassung.Controllers.Home.Interfaces;
using VTAuftragserfassung.Controllers.Login;
using VTAuftragserfassung.Controllers.Login.Interfaces;
using VTAuftragserfassung.Database.Connection;
using VTAuftragserfassung.Database.Connection.Interfaces;
using VTAuftragserfassung.Database.DataAccess;
using VTAuftragserfassung.Database.DataAccess.Interfaces;
using VTAuftragserfassung.Database.DataAccess.Services;
using VTAuftragserfassung.Database.DataAccess.Services.Interfaces;
using VTAuftragserfassung.Database.Repository;
using VTAuftragserfassung.Database.Repository.Interfaces;

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
            ResourceManager resM = new("VTAuftragserfassung.Database.DataAccess.MSSqlQueries", Assembly.GetExecutingAssembly());

            builder.Services.AddSingleton(resM);
            builder.Services.AddSingleton<ISqlConnector>(conn => new SqlConnector(connectionString));
            builder.Services.AddScoped<ICachingService, CachingService>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IDatabaseAccess, DatabaseAccess>();
            builder.Services.AddScoped<IDatabaseService, DatabaseService>();
            builder.Services.AddScoped<ILoginRepository, LoginRepository>();
            builder.Services.AddScoped<IHomeRepository, HomeRepository>();
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
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddResponseCompression();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = "VTA.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(1);
                options.Cookie.IsEssential = true;
             
            });
            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AutomaticAuthentication = false;
            });
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
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                });

            #endregion Services

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Dashboard}/{id?}");

            app.MapControllerRoute(
                name: "Login",
                pattern: "{controller=Login}/{action=Index}/{id?}");



            app.Run();
        }

        #endregion Public Methods
    }
}