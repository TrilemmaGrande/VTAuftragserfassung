using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using VTAuftragserfassung.Controllers.Home;
using VTAuftragserfassung.Controllers.Login;
using VTAuftragserfassung.Database.Connection;
using VTAuftragserfassung.Database.DataAccess;
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
            builder.Services.AddScoped<IDataAccess<IDatabaseObject>, DatabaseAccess>();
            builder.Services.AddScoped<IDbRepository, DbRepository>();
            builder.Services.AddScoped<ILoginLogic, LoginLogic>();
            builder.Services.AddScoped<IHomeLogic, HomeLogic>();
            builder.Services.AddControllersWithViews();
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

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseHsts();
            }
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