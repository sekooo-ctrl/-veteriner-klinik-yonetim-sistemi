
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VeterinerKlinik.Web.Models;

namespace VeterinerKlinik.Web.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
        ) : base(options)
        {
        }

        public DbSet<EvcilHayvan> EvcilHayvanlar { get; set; }
        public DbSet<Veteriner> Veterinerler { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<AsiKaydi> AsiKayitlari { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
    }
}
