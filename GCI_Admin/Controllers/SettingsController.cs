using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
