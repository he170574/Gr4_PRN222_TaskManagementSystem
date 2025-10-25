using CMS2.Data;
using CMS2.Services;
using Microsoft.EntityFrameworkCore;

namespace CMS2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // K?t n?i ??n SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // C?u hình Razor Pages
            builder.Services.AddRazorPages();

            // ??ng ký EmailService
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // C?u hình Session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // X? lý l?i
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Kích ho?t session
            app.UseSession();

            app.UseAuthorization();

            // Ánh x? Razor Pages
            app.MapRazorPages();

            app.Run();
        }
    }
}
