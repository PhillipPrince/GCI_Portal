using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class AnnouncementsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
