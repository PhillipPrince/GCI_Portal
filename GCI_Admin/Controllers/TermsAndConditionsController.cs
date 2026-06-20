using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class TermsAndConditionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
