using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class EldersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
