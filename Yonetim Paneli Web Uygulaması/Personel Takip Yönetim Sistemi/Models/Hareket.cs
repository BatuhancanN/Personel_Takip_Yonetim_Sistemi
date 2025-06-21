using System.ComponentModel.DataAnnotations.Schema;

namespace Personel_Takip_Yönetim_Sistemi.Models
{
    public class Hareket
    {
        [Column("h_id")]  // Veritabanındaki sütun adı
        public int HareketId { get; set; }

        [Column("h_personel_id")]  // Veritabanındaki sütun adı
        public int PersonelId { get; set; }

        [Column("h_durumu")]  // Veritabanındaki sütun adı
        public bool HareketDurumu { get; set; }

        [Column("h_tarih")]  // Veritabanındaki sütun adı
        public string HareketTarihi { get; set; }  // string olarak değiştirildi

        public Personel Personel { get; set; }
    }
}
