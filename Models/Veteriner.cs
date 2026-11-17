using System.ComponentModel.DataAnnotations;

namespace VeterinerKlinik.Web.Models
{
    public class Veteriner
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Veteriner adı zorunludur.")]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Veteriner soyadı zorunludur.")]
        [StringLength(100)]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
        [StringLength(100)]
        public string UzmanlikAlani { get; set; } = string.Empty;

        [Required]
        public string KullaniciId { get; set; } = string.Empty;
    }
}
