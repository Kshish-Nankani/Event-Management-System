using EventManagementSystem.Data;
using EventManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 🔹 Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                )
            );

            // 🔹 Session (IMPORTANT FOR AUTH)
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // 🔹 MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // 🔹 Seed Admin user (if not exists)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Check if any Admin exists
                if (!db.Users.Any(u => u.Role == "Admin"))
                {
                    db.Users.Add(new User
                    {
                        FullName = "Admin",
                        Email = "admin@example.com", // change as needed
                        Password = "admin123",       // change as needed
                        Role = "Admin"
                    });
                    db.SaveChanges();
                }
            }

            // 🔹 Error handling
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 🔹 Session MUST be before Authorization
            app.UseSession();
            app.UseAuthorization();

            // 🔹 Default route → Login
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}"
            );

            app.Run();
        }
    }
}
