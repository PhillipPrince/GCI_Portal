using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class AuditLogsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
