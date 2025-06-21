using System.ComponentModel.DataAnnotations;

namespace Personel_Takip_Yönetim_Sistemi.Models.ViewModels
{
    public class LoginEditViewModel
    {
        public int login_id { get; set; }
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string loginame { get; set; }
        [Required(ErrorMessage = "İsim zorunludur.")]
        public string login_isim { get; set; }
        [Required(ErrorMessage = "Soyisim zorunludur.")]
        public string login_soyisim { get; set; }
        [Required(ErrorMessage = "Rol zorunludur.")]
        public int login_role { get; set; }
        public string YeniSifre { get; set; } 
    }
}
