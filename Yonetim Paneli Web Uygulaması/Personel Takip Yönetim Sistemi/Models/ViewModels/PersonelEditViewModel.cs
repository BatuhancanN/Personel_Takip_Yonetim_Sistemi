using System.ComponentModel.DataAnnotations;

namespace Personel_Takip_Yönetim_Sistemi.Models.ViewModels
{
    public class PersonelEditViewModel
    {
        public int PersonelId { get; set; }
        [Required(ErrorMessage = "Personel adı zorunludur.")]
        public string PersonelIsim { get; set; }
        [Required(ErrorMessage = "Personel soyadı zorunludur.")]
        public string PersonelSoyisim { get; set; }
        [Required(ErrorMessage = "Personel pozisyonu zorunludur.")]
        public string PersonelPozisyon { get; set; }
    }
}
