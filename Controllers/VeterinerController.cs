
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinerKlinik.Web.Data;
using VeterinerKlinik.Web.Models;
using VeterinerKlinik.Web.ViewModels;

namespace VeterinerKlinik.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VeterinerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public VeterinerController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Veteriner
        // Veterinerleri listele
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var veterinerler = await _context.Veterinerler
                .AsNoTracking()
                .OrderBy(v => v.Ad)
                .ThenBy(v => v.Soyad)
                .ToListAsync();

            return View(veterinerler);
        }

        // GET: Veteriner/Create
        // Yeni veteriner ekleme formu
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Veteriner/Create
        // Yeni veteriner ve kullanıcı hesabı oluştur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            VeterinerEkleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var mevcutKullanici =
                await _userManager.FindByEmailAsync(model.Email);

            if (mevcutKullanici != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Bu e-posta adresi zaten kullanılıyor.");

                return View(model);
            }

            var kullanici = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var sonuc = await _userManager.CreateAsync(
                kullanici, model.Password);

            if (!sonuc.Succeeded)
            {
                foreach (var hata in sonuc.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty, hata.Description);
                }

                return View(model);
            }

            // Veteriner rolünü ata
            var rolSonucu = await _userManager.AddToRoleAsync(
                kullanici, "Veteriner");

            if (!rolSonucu.Succeeded)
            {
                await _userManager.DeleteAsync(kullanici);

                ModelState.AddModelError(
                    string.Empty,
                    "Veteriner rolü atanamadı.");

                return View(model);
            }

            // Veteriner kaydını oluştur
            var veteriner = new Veteriner
            {
                Ad = model.Ad,
                Soyad = model.Soyad,
                UzmanlikAlani = model.UzmanlikAlani,
                KullaniciId = kullanici.Id
            };

            try
            {
                _context.Veterinerler.Add(veteriner);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                _context.Entry(veteriner).State =
                    EntityState.Detached;

                await _userManager.DeleteAsync(kullanici);

                ModelState.AddModelError(
                    string.Empty,
                    "Veteriner kaydedilemedi. Tekrar deneyin.");

                return View(model);
            }

            TempData["Success"] =
                "Veteriner başarıyla eklendi.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Veteriner/Edit/1
        // Düzenleme formunu aç
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var veteriner = await _context.Veterinerler
                .FindAsync(id);

            if (veteriner == null)
                return NotFound();

            var model = new VeterinerDuzenleViewModel
            {
                Id = veteriner.Id,
                Ad = veteriner.Ad,
                Soyad = veteriner.Soyad,
                UzmanlikAlani = veteriner.UzmanlikAlani
            };

            return View(model);
        }

        // POST: Veteriner/Edit/1
        // Veteriner bilgilerini güncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, VeterinerDuzenleViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var veteriner = await _context.Veterinerler
                .FindAsync(id);

            if (veteriner == null)
                return NotFound();

            veteriner.Ad = model.Ad;
            veteriner.Soyad = model.Soyad;
            veteriner.UzmanlikAlani = model.UzmanlikAlani;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Veteriner bilgileri başarıyla güncellendi.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Veteriner/Delete/1
        // Silme onay sayfası
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var veteriner = await _context.Veterinerler
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            if (veteriner == null)
                return NotFound();

            return View(veteriner);
        }

        // POST: Veteriner/Delete/1
        // Veteriner ve giriş hesabını sil
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var veteriner = await _context.Veterinerler
                .FindAsync(id);

            if (veteriner == null)
                return NotFound();

            // Veterinerin randevusu var mı?
            bool randevuVar = await _context.Randevular
                .AnyAsync(r => r.VeterinerId == id);

            if (randevuVar)
            {
                TempData["Error"] =
                    "Bu veterinerin randevu kayıtları bulunduğu " +
                    "için silinmesine izin verilmiyor.";

                return RedirectToAction(nameof(Index));
            }

            // Veterinere bağlı kullanıcı hesabını bul
            var kullanici = await _userManager
                .FindByIdAsync(veteriner.KullaniciId);

            // Aynı veritabanındaki işlemleri birlikte yürüt
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Veterinerler.Remove(veteriner);
                await _context.SaveChangesAsync();

                if (kullanici != null)
                {
                    var sonuc =
                        await _userManager.DeleteAsync(kullanici);

                    if (!sonuc.Succeeded)
                    {
                        throw new InvalidOperationException(
                            "Veterinerin kullanıcı hesabı silinemedi.");
                    }
                }

                await transaction.CommitAsync();

                TempData["Success"] =
                    "Veteriner ve giriş hesabı başarıyla silindi.";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                TempData["Error"] =
                    "Silme işlemi sırasında hata oluştu.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
