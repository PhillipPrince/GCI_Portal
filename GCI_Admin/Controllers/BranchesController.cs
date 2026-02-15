using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class BranchesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
