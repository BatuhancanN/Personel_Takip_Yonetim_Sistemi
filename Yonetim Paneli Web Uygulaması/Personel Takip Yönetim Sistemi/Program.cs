using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

using Personel_Takip_Y�netim_Sistemi.Data;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s�n� yap�land�r�yoruz
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
    builder.Configuration.GetConnectionString("DefaultConnectionMySQL"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnectionMySQL"))
));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Error/AccessDenied";
    });



// Di�er servisleri ekliyoruz
builder.Services.AddControllersWithViews();

var app = builder.Build();

// HTTP istek i�leme hatt�n� yap�land�r�yoruz
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication(); // BU �OK �NEML�, Yetkilendirme �ncesinde olmal�!

app.UseRouting();
app.UseAuthentication(); // Kimlik do�rulama �nce gelmeli
app.UseAuthorization(); // Yetkilendirme sonra gelmeli
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
