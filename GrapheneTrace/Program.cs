using GrapheneTrace.Data;               // access to AppDbContext and DbSeeder
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC controllers and views.
builder.Services.AddControllersWithViews();

// Use session so we can remember who is logged in (user ID, role, name).
builder.Services.AddSession();

// Register AppDbContext and configure it to use SQL Server.
// Connection string lives in appsettings.json under "GrapheneTraceConnection".
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("GrapheneTraceConnection")));

var app = builder.Build();

// ✅ Seed demo data on startup (only if DB is empty).
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(context);
}

// Configure HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Session must run before authorization and endpoints.
app.UseSession();

app.UseAuthorization();

// Static files (css, js, images) and other assets.
app.MapStaticAssets();

// Default MVC route: /Home/Index by default.
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Start the application.
app.Run();
