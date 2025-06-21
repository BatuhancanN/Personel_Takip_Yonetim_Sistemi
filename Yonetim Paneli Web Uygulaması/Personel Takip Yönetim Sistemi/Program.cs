using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

using Personel_Takip_Yönetim_Sistemi.Data;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsýný yapýlandýrýyoruz
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



// Diðer servisleri ekliyoruz
builder.Services.AddControllersWithViews();

var app = builder.Build();

// HTTP istek iþleme hattýný yapýlandýrýyoruz
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication(); // BU ÇOK ÖNEMLÝ, Yetkilendirme öncesinde olmalý!

app.UseRouting();
app.UseAuthentication(); // Kimlik doðrulama önce gelmeli
app.UseAuthorization(); // Yetkilendirme sonra gelmeli
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
