using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Personel_Takip_Yönetim_Sistemi.Models
{
    [Table("login")]
    public class Login
    {

        [Key]
        public int login_id { get; set; }
        [Column("loginame")]

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        public string loginame { get; set; }
        [Column("login_isim")]
        [Required(ErrorMessage = "İsim zorunludur")]
        public string login_isim { get; set; }
        [Column("login_soyisim")]
        [Required(ErrorMessage = "Soyisim zorunludur")]
        public string login_soyisim { get; set; }
        [Column("login_pass")]
        [Required(ErrorMessage = "Şifre zorunludur")]
        public string login_pass { get; set; }
        [Column("login_role")]
        [Required(ErrorMessage = "Yetki zorunludur")]
        public int login_role { get; set; }

    }
}
