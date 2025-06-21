namespace Personel_Takip_Yönetim_Sistemi.Models.ViewModels
{
    public class HareketViewModel
    {
        public int HareketId { get; set; }
        public int PersonelId { get; set; }
        public string PersonelAdSoyad { get; set; } // Ad + Soyad
        public bool HareketDurumu { get; set; }
        public string HareketTarihi { get; set; }
    }
}
