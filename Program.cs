using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using VirtualQueueSystem.Data;
using VirtualQueueSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load Configuration
var configuration = builder.Configuration;

// ✅ Register AppDbContext using SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=VirtualQueueSystem.db"));

// ✅ Enable authentication with cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddAuthorization();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Add MVC and Controllers
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ✅ Apply Database Migrations & Seed Data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        dbContext.Database.Migrate(); // Apply pending migrations

        // ✅ Seed Admin User
        if (!dbContext.Users.Any(u => u.Username == "admin"))
        {
            dbContext.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Change password if needed
                Role = "Admin"
            });

            dbContext.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration error: {ex.Message}");
    }
}

// ✅ Configure Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Define Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "user",
    pattern: "User/{action=Dashboard}/{id?}",
    defaults: new { controller = "User", action = "Dashboard" });

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Dashboard}/{id?}",
    defaults: new { controller = "Admin", action = "Dashboard" });

app.MapControllerRoute(
    name: "menu",
    pattern: "User/Menu",
    defaults: new { controller = "User", action = "Menu" });

app.MapControllerRoute(
    name: "reservation",
    pattern: "User/ReservationDetails",
    defaults: new { controller = "User", action = "ReservationDetails" });

app.MapControllerRoute(
    name: "checkout",
    pattern: "User/Checkout",
    defaults: new { controller = "User", action = "Checkout" });

app.MapControllerRoute(
    name: "user_feedback",
    pattern: "User/Feedback",
    defaults: new { controller = "User", action = "Feedback" }
);



app.MapControllerRoute(
    name: "manageFeedback",
    pattern: "Admin/ManageFeedback",
    defaults: new { controller = "Feedback", action = "ManageFeedback" });

app.Run();
