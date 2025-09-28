using AutoStats.Data;
using AutoStats.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Dodajem konekciju prema SQLite bazi
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' nije pronađena.");

builder.Services.AddDbContext<AutoStatsContext>(options =>
    options.UseSqlite(connectionString));

// Konfigurišem Identity sa prilagođenim korisničkim klasama
builder.Services.AddDefaultIdentity<AutoStatsUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // ili true ako koristim potvrdu emaila
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AutoStatsContext>();

// Dodajem kontrolere i Razor Pages podršku
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Dodajem autentifikaciju
app.UseAuthorization();  // Dodajem autorizaciju

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Bitno za Identity stranice

app.Run();

