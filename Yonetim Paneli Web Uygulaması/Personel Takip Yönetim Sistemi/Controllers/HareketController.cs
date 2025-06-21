using Microsoft.AspNetCore.Mvc;
using Personel_Takip_Yönetim_Sistemi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Personel_Takip_Yönetim_Sistemi.Data;

namespace Test3.Controllers
{
    [Authorize]
    public class HareketController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HareketController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var sqlQuery = @"
                            SELECT
                                h.h_id AS HareketId,
                                h.h_personel_id AS PersonelId,
                                CONCAT(p.personel_isim, ' ', p.personel_soyisim) AS PersonelAdSoyad,
                                h.h_durumu AS HareketDurumu,
                                DATE_FORMAT(STR_TO_DATE(h.h_tarih, '%Y-%m-%d %H:%i:%s'), '%Y-%m-%d %H:%i:%s') AS HareketTarihi
                            FROM Hareketler h
                            INNER JOIN Personeller p ON h.h_personel_id = p.personel_id
                            ORDER BY STR_TO_DATE(h.h_tarih, '%Y-%m-%d %H:%i:%s') DESC
            ";


            var hareketler = _db.HareketViewModels
                .FromSqlRaw(sqlQuery)
                .ToList();

            return View(hareketler);
        }

        public JsonResult GetLatestHareketler()
        {
            var hareketler = _db.Hareketler
                .Include(h => h.Personel) // Navigation property varsa
                .Where(h => h.HareketTarihi != null)
                .OrderByDescending(h => h.HareketTarihi)
                .Select(h => new
                {
                    hareketId = h.HareketId,
                    hareketDurumu = h.HareketDurumu,
                    hareketTarihi = h.HareketTarihi,
                    personelAdSoyad = h.Personel != null
                        ? h.Personel.PersonelIsim + " " + h.Personel.PersonelSoyisim
                        : "Bilinmiyor"
                })
                .ToList();

            return Json(hareketler);
        }

        [HttpGet]
        public JsonResult GetHareketlerByName(string isim)
        {
            var hareketler = _db.Hareketler
                .Include(h => h.Personel)
                .Where(h => h.HareketTarihi != null &&
                            (h.Personel.PersonelIsim + " " + h.Personel.PersonelSoyisim)
                                .Contains(isim)) // isim eşleşmesi yap
                .OrderByDescending(h => h.HareketTarihi)
                .Select(h => new
                {
                    hareketId = h.HareketId,
                    hareketDurumu = h.HareketDurumu,
                    hareketTarihi = h.HareketTarihi,
                    personelAdSoyad = h.Personel != null
                        ? h.Personel.PersonelIsim + " " + h.Personel.PersonelSoyisim
                        : "Bilinmiyor"
                })
                .ToList();

            return Json(hareketler);
        }


    }

}
