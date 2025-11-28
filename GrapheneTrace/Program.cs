using System;
using GrapheneTrace.Data;
using GrapheneTrace.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. Database
// ---------------------------------------------------------

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ---------------------------------------------------------
// 2. MVC + Session + CSV heat data service
// ---------------------------------------------------------

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IHeatDataService, CsvHeatDataService>();

builder.Services.AddControllersWithViews();

// ---------------------------------------------------------
// 3. Build app
// ---------------------------------------------------------

var app = builder.Build();

// ---------------------------------------------------------
// 4. Seed demo users (5 patient accounts)
// ---------------------------------------------------------

DemoSeeder.Seed(app.Services);

// ---------------------------------------------------------
// 5. HTTP pipeline
// ---------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
