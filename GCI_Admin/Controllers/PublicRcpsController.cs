using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GCI_Admin.Controllers
{
    [AllowAnonymous]
    public class PublicRcpsController : Controller
    {
        private readonly IRcpsService _rcpsService;

        public PublicRcpsController(IRcpsService rcpsService)
        {
            _rcpsService = rcpsService;
        }

        [HttpGet("Rcps/Contribute/{code}")]
        public async Task<IActionResult> Contribute(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return NotFound("Invalid invite code.");
            }

            var result = await _rcpsService.GetRcpsInviteByCodeAsync(code);

            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound("This invite link does not exist or has been removed.");
            }

            return View(result.Data);
        }
    }
}
