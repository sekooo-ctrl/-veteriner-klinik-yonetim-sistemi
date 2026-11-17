using System;
using System.ComponentModel.DataAnnotations;

namespace VeterinerKlinik.Web.Models
{
    public class AsiKaydi
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Aşı adı zorunludur.")]
        [StringLength(100)]
        public string AsiAdi { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime YapilmaTarihi { get; set; }

        [DataType(DataType.Date)]
        public DateTime? SonrakiDozTarihi { get; set; }

        [Required]
        public int EvcilHayvanId { get; set; }

        public EvcilHayvan EvcilHayvan { get; set; } = null!;
    }
}
