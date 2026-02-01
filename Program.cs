using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using testPortfolio.Controllers;
using testPortfolio.Middleware;
using testPortfolio.Services;

namespace testPortfolio
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.  

            builder.Services.Configure<MongoSettings>(
            builder.Configuration.GetSection("MongoDbSettings"));

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

                // Activer le logging sensible uniquement en développement
                if (builder.Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                }
            });

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();

            // Sécurité des cookies adaptée à l'environnement
            var cookieSecurePolicy = builder.Environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest  // HTTP autorisé en dev
                : CookieSecurePolicy.Always;        // HTTPS obligatoire en prod

            // Configuration sécurisée des cookies d'authentification
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = cookieSecurePolicy;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            // Configuration des cookies anti-forgery
            builder.Services.AddAntiforgery(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = cookieSecurePolicy;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            builder.Services.AddSingleton<ProductService>();
            builder.Services.AddSingleton<PictureService>();

            builder.Services.AddScoped<ProductController>();
            builder.Services.AddScoped<BackOffice>();
            builder.Services.AddScoped<PictureController>();
            builder.Services.AddScoped<HomeController>();

            var app = builder.Build();
            app.MapRazorPages();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Gestion des pages d'erreur par code de statut (404, 500, etc.)
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            // Security Headers (X-Content-Type-Options, X-Frame-Options, CSP, etc.)
            app.UseSecurityHeaders();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name : "backOffice",
                pattern: "{controller=BackOffice}/{action=Index}"
            );

            app.Run();
        }
    }
}
