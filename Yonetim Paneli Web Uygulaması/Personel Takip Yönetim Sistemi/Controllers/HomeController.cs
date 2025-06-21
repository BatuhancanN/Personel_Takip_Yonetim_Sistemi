using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Personel_Takip_Yönetim_Sistemi.Models;
using Personel_Takip_Yönetim_Sistemi.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Personel_Takip_Yönetim_Sistemi.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var girisYapanPersoneller = await _db.Personeller
                .Where(p => p.PersonelDurum == true)
                .ToListAsync();

            ViewData["AktifGirisYapanSayisi"] = girisYapanPersoneller.Count;

            return View(girisYapanPersoneller);
        }


        public async Task<JsonResult> GetLatestGirisYapanPersoneller()
        {
            // Sadece PersonelDurum'u true olanları alıyoruz
            var girisYapanPersoneller = await _db.Personeller
                .Where(p => p.PersonelDurum == true)
                .Select(p => new
                {
                    personelId = p.PersonelId,
                    personelAdSoyad = p.PersonelIsim.ToUpper() + " " + p.PersonelSoyisim.ToUpper(),
                    personelPozisyon = p.PersonelPozisyon.ToUpper(),
                })
                .ToListAsync();

            var result = new
            {
                personeller = girisYapanPersoneller,
                count = girisYapanPersoneller.Count
            };

            return Json(result);
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
