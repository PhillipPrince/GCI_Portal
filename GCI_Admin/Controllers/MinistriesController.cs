using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class MinistriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
