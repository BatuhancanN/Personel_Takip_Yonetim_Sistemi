using Microsoft.AspNetCore.Mvc;
using Personel_Takip_Yönetim_Sistemi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Personel_Takip_Yönetim_Sistemi.Data;
using Microsoft.AspNetCore.Authorization;
using Personel_Takip_Yönetim_Sistemi.Models.ViewModels;
using QRCoder;

namespace Personel_Takip_Yönetim_Sistemi.Controllers
{
    [Authorize]
    public class PersonelController : Controller
    {
        private readonly ApplicationDbContext _db;

        public PersonelController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var personeller = _db.Personeller
                .Where(p => p.PersonelPozisyon != "x")
                .OrderBy(p => p.PersonelIsim)
                .ThenBy(p => p.PersonelSoyisim)
                .ToList();

            return View(personeller);
        }

        public JsonResult GetLatestPersoneller()
        {
            var sqlQuery = @"
                            SELECT personel_id, personel_isim, personel_soyisim, personel_pozisyon, personel_durum
                            FROM Personeller
                            WHERE personel_pozisyon != 'x'
                            ORDER BY personel_isim, personel_soyisim";

            var personeller = _db.Personeller
                .FromSqlRaw(sqlQuery)
                .ToList();

            return Json(personeller);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            var personel = _db.Personeller.FirstOrDefault(p => p.PersonelId == id);
            if (personel == null)
            {
                return NotFound();
            }

            var viewModel = new PersonelEditViewModel
            {
                PersonelId = personel.PersonelId,
                PersonelIsim = personel.PersonelIsim,
                PersonelSoyisim = personel.PersonelSoyisim,
                PersonelPozisyon = personel.PersonelPozisyon
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(PersonelEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var personel = _db.Personeller.FirstOrDefault(p => p.PersonelId == model.PersonelId);
            if (personel == null)
                return NotFound();

            personel.PersonelIsim = model.PersonelIsim;
            personel.PersonelSoyisim = model.PersonelSoyisim;
            personel.PersonelPozisyon = model.PersonelPozisyon;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var personel = _db.Personeller.FirstOrDefault(p => p.PersonelId == id);
            if (personel == null)
            {
                return NotFound();
            }

            personel.PersonelPozisyon = "x";
            personel.PersonelDurum = false;
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult QRCode(int id)
        {
            var personel = _db.Personeller.FirstOrDefault(p => p.PersonelId == id);
            if (personel == null)
            {
                return NotFound();
            }

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode($"{personel.PersonelId}", QRCodeGenerator.ECCLevel.Q);

            using var qrCode = new QRCoder.BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsPng = qrCode.GetGraphic(20); // 20 → daha büyük boyut

            // PNG formatında, "attachment" olarak dosya ismiyle sun
            return File(qrCodeAsPng, "image/png", $"{personel.PersonelIsim}_{personel.PersonelSoyisim}_qrcode.png");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Personel personel)
        {
            // Bu satırla gelen verileri kontrol et
            Console.WriteLine($"İsim: {personel.PersonelIsim}, Soyisim: {personel.PersonelSoyisim}, Pozisyon: {personel.PersonelPozisyon}");

                personel.PersonelDurum = true;
                _db.Personeller.Add(personel);
                _db.SaveChanges();
                return RedirectToAction("Index");

            return View(personel);
        }

        [HttpGet]
        public JsonResult GetPersonellerByName(string isim)
        {
            var personeller = _db.Personeller
                .Where(p => (p.PersonelIsim + " " + p.PersonelSoyisim).Contains(isim))
                .Select(p => new {
                    p.PersonelId,
                    p.PersonelIsim,
                    p.PersonelSoyisim,
                    p.PersonelPozisyon,
                    p.PersonelDurum
                })
                .ToList();

            return Json(personeller);
        }


    }
}
