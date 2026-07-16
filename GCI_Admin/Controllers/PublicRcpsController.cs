using GCI_Admin.DBOperations;
using GCI_Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GCI_Admin.Controllers
{
    [AllowAnonymous]
    public class PublicRcpsController : Controller
    {
        private readonly AppDbContext _context;

        public PublicRcpsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Rcps/Contribute/{code}")]
        public async Task<IActionResult> Contribute(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return NotFound("Invalid invite code.");
            }

            var invite = await _context.RcpsInvites
                .Include(i => i.RcpsPlan)
                .ThenInclude(p => p.Rcps)
                .Include(i => i.Member)
                .FirstOrDefaultAsync(i => i.UniqueLinkCode == code);

            if (invite == null)
            {
                return NotFound("This invite link does not exist or has been removed.");
            }

            return View(invite);
        }
    }
}
