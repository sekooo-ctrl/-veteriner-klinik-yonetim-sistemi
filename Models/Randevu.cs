
using System;
using System.ComponentModel.DataAnnotations;

namespace VeterinerKlinik.Web.Models
{
    public enum RandevuDurumu
    {
        Bekliyor,
        Onaylandi,
        Reddedildi,
        Tamamlandi,
        IptalEdildi
    }

    public class Randevu
    {
        public int Id { get; set; }

        [Required]
        public int EvcilHayvanId { get; set; }

        public EvcilHayvan EvcilHayvan { get; set; } = null!;

        [Required]
        public int VeterinerId { get; set; }

        public Veteriner Veteriner { get; set; } = null!;

        [Required]
        public int HizmetId { get; set; }

        public Hizmet Hizmet { get; set; } = null!;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime RandevuTarihi { get; set; }

        [Range(1, 480)]
        public int SureDakika { get; set; }

        [Range(0, 100000)]
        public decimal Ucret { get; set; }

        public RandevuDurumu Durum { get; set; }
            = RandevuDurumu.Bekliyor;

        [StringLength(2000)]
        public string? MuayeneNotu { get; set; }
    }
}
