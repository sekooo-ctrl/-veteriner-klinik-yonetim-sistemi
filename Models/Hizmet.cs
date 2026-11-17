using System.ComponentModel.DataAnnotations;

namespace VeterinerKlinik.Web.Models
{
    public class Hizmet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı zorunludur.")]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Aciklama { get; set; }

        [Range(1, 480, ErrorMessage = "Süre 1-480 dakika arasında olmalıdır.")]
        public int SureDakika { get; set; }

        [Range(0.01, 100000, ErrorMessage = "Geçerli bir ücret giriniz.")]
        public decimal Ucret { get; set; }
    }
}
