
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinerKlinik.Web.Data;

namespace VeterinerKlinik.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.HayvanSayisi =
                await _context.EvcilHayvanlar.CountAsync();

            ViewBag.VeterinerSayisi =
                await _context.Veterinerler.CountAsync();

            ViewBag.RandevuSayisi =
                await _context.Randevular.CountAsync();

            ViewBag.HizmetSayisi =
                await _context.Hizmetler.CountAsync();

            return View();
        }
    }
}
