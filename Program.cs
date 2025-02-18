using Microsoft.EntityFrameworkCore;
using MyVideostore.Data;
using Microsoft.AspNetCore.Identity;
using MyVideostore.Areas.Identity.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register the MyVideostoreDbContext with SQL Server (use your actual connection string)
builder.Services.AddDbContext<MyVideostoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the ApplicationDbContext with SQL Server (use your actual connection string)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<MyVideostoreDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Map controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
