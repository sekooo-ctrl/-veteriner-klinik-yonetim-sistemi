
using System.ComponentModel.DataAnnotations;

namespace VeterinerKlinik.Web.ViewModels
{
    public class VeterinerDuzenleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(100)]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
        [StringLength(100)]
        public string UzmanlikAlani { get; set; } = string.Empty;
    }
}
