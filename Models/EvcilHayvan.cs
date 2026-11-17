
using System;
using System.ComponentModel.DataAnnotations;

namespace VeterinerKlinik.Web.Models
{
    public class EvcilHayvan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hayvan adı zorunludur.")]
        [StringLength(100)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hayvan türü zorunludur.")]
        [StringLength(50)]
        public string Tur { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Irk { get; set; }

        [DataType(DataType.Date)]
        public DateTime DogumTarihi { get; set; }

        [Range(0.01, 500, ErrorMessage = "Geçerli bir kilo giriniz.")]
        public decimal Kilo { get; set; }

        [Required]
        public string SahipId { get; set; } = string.Empty;
    }
}
