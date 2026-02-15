using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class NotificationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
