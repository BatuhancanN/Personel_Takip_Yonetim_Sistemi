using Personel_Takip_Yönetim_Sistemi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Personel_Takip_Yönetim_Sistemi.Models
{
    public class Personel
    {

        [Column("personel_id")]
        public int PersonelId { get; set; }

        [Required(ErrorMessage = "İsim zorunludur")]
        [Column("personel_isim")]
        public string PersonelIsim { get; set; }

        [Required(ErrorMessage = "Soyisim zorunludur")]
        [Column("personel_soyisim")]
        public string PersonelSoyisim { get; set; }

        [Required(ErrorMessage = "Pozisyon zorunludur")]
        [Column("personel_pozisyon")]
        public string PersonelPozisyon { get; set; }

        [Column("personel_durum")]
        public bool PersonelDurum { get; set; }

        public List<Hareket> Hareketler { get; set; }

    }
}
