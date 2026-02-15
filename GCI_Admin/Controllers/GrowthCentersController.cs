using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class GrowthCentersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
