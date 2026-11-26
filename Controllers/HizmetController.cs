
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinerKlinik.Web.Data;
using VeterinerKlinik.Web.Models;

namespace VeterinerKlinik.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HizmetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HizmetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hizmetleri listele
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var hizmetler = await _context.Hizmetler
                .AsNoTracking()
                .OrderBy(h => h.Ad)
                .ToListAsync();

            return View(hizmetler);
        }

        // Hizmet ekleme formu
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Yeni hizmet kaydet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Ad,Aciklama,SureDakika,Ucret")] Hizmet hizmet)
        {
            if (!ModelState.IsValid)
                return View(hizmet);

            _context.Hizmetler.Add(hizmet);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Hizmet başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // Düzenleme formu
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);

            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        // Hizmet bilgilerini güncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Ad,Aciklama,SureDakika,Ucret")] Hizmet model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var hizmet = await _context.Hizmetler.FindAsync(id);

            if (hizmet == null)
                return NotFound();

            hizmet.Ad = model.Ad;
            hizmet.Aciklama = model.Aciklama;
            hizmet.SureDakika = model.SureDakika;
            hizmet.Ucret = model.Ucret;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Hizmet başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // Silme onay sayfası
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var hizmet = await _context.Hizmetler
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hizmet == null)
                return NotFound();

            return View(hizmet);
        }

        // Hizmeti sil
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);

            if (hizmet == null)
                return NotFound();

            // Randevuda kullanılmış hizmetleri silmeyelim
            bool randevuVar = await _context.Randevular
                .AnyAsync(r => r.HizmetId == id);

            if (randevuVar)
            {
                TempData["Error"] =
                    "Bu hizmete ait randevular bulunduğu için silinemez.";

                return RedirectToAction(nameof(Index));
            }

            _context.Hizmetler.Remove(hizmet);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Hizmet başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
