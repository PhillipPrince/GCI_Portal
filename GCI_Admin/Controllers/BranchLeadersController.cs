using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class BranchLeadersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
