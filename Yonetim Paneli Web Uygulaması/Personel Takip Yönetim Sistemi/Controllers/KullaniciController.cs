using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Personel_Takip_Yönetim_Sistemi.Data;
using Personel_Takip_Yönetim_Sistemi.Models;
using Personel_Takip_Yönetim_Sistemi.Models.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Personel_Takip_Yönetim_Sistemi.Controllers
{
    [Authorize]
    public class KullaniciController : Controller
    {
        private readonly ILogger<KullaniciController> _logger;
        private readonly ApplicationDbContext _db;

        public KullaniciController(ILogger<KullaniciController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var kullanicilar = await _db.Logins
                .Where(k => k.login_role != -1)
                .ToListAsync();
            return View(kullanicilar);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);
            var currentUserRole = int.Parse(User.FindFirst(ClaimTypes.Role).Value);

            var kullanici = await _db.Logins.FirstOrDefaultAsync(k => k.login_id == id);
            if (kullanici == null)
                return NotFound();

            // Kendisi ise her zaman düzenleyebilir
            if (currentUserId != id)
            {
                // 1. rol kimseyi düzenleyemez (sadece kendini)
                if (currentUserRole == 1)
                    return Forbid();

                // Daha düşük rol, daha yüksek rolü düzenleyemez
                if (currentUserRole <= kullanici.login_role)
                    return Forbid();
            }

            var model = new LoginEditViewModel
            {
                login_id = kullanici.login_id,
                loginame = kullanici.loginame,
                login_isim = kullanici.login_isim,
                login_soyisim = kullanici.login_soyisim,
                login_role = kullanici.login_role
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoginEditViewModel model)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);
            var currentUserRole = int.Parse(User.FindFirst(ClaimTypes.Role).Value);

            var hedefKullanici = await _db.Logins.FindAsync(id);
            if (hedefKullanici == null)
                return NotFound();

            if (id != model.login_id)
                return NotFound();

            // Kendisi değilse kontrol et
            if (currentUserId != id)
            {
                // Rol 1 olan kimseyi düzenleyemez
                if (currentUserRole == 1)
                    return Forbid();

                // Daha düşük veya eşit rol, daha yüksek ya da eşit rolü düzenleyemez
                if (currentUserRole <= hedefKullanici.login_role)
                    return Forbid();
            }

            // Şifre kısmı boşsa validasyon engeli olmasın
            if (ModelState.ContainsKey("YeniSifre"))
            {
                ModelState["YeniSifre"].Errors.Clear();
                ModelState["YeniSifre"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    hedefKullanici.login_isim = model.login_isim;
                    hedefKullanici.login_soyisim = model.login_soyisim;
                    hedefKullanici.loginame = model.loginame;
                    hedefKullanici.login_role = model.login_role;

                    if (!string.IsNullOrEmpty(model.YeniSifre))
                        hedefKullanici.login_pass = model.YeniSifre;

                    _db.Update(hedefKullanici);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_db.Logins.Any(k => k.login_id == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);
            var currentUserRole = int.Parse(User.FindFirst(ClaimTypes.Role).Value);

            var hedefKullanici = await _db.Logins.FindAsync(id);
            if (hedefKullanici == null)
                return NotFound();

            // Kendini silemez
            if (currentUserId == id)
                return Forbid();

            // 1. rütbe kimseyi silemez
            if (currentUserRole == 1)
                return Forbid();

            // Düşük rütbe, yüksek rütbeyi silemez
            if (currentUserRole < hedefKullanici.login_role)
                return Forbid();

            try
            {
                hedefKullanici.login_role = -1; // Mantıksal silme
                _db.Update(hedefKullanici);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500, "Kullanıcı silinirken bir hata oluştu.");
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Login model)
        {
            if (ModelState.IsValid)
            {
                // SHA256 ile şifreleme
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(model.login_pass);
                    byte[] hashBytes = sha256.ComputeHash(bytes);
                    StringBuilder sb = new StringBuilder();

                    foreach (byte b in hashBytes)
                    {
                        sb.Append(b.ToString("x2")); // Hex format
                    }

                    model.login_pass = sb.ToString();
                }

                // Veritabanına ekle
                _db.Logins.Add(model);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
