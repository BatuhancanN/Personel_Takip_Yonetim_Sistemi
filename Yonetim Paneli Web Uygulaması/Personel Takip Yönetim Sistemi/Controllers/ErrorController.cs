using Microsoft.AspNetCore.Mvc;

namespace Personel_Takip_Yönetim_Sistemi.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

}
