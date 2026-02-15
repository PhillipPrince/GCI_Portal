using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class AssembliesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
