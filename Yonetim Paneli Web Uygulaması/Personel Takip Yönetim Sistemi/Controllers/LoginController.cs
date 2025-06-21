using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Personel_Takip_Yönetim_Sistemi.Models;
using Personel_Takip_Yönetim_Sistemi.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Personel_Takip_Yönetim_Sistemi.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Login model)
        {
            // Şifreyi hashleyerek karşılaştır
            string hashedPassword = ComputeSha256Hash(model.login_pass);

            var user = _context.Logins
                .FirstOrDefault(u => u.loginame == model.loginame && u.login_pass == hashedPassword);

            if (user != null)
            {
                if (user.login_role == -1)
                {
                    ViewBag.Error = "Hesabınız devre dışı bırakılmıştır. Lütfen sistem yöneticisiyle iletişime geçin.";
                    return View();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.loginame),
                    new Claim("FullName", $"{user.login_isim} {user.login_soyisim}"),
                    new Claim(ClaimTypes.Role, user.login_role.ToString()),
                    new Claim("UserId", user.login_id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

        // SHA-256 Hash Fonksiyonu
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
