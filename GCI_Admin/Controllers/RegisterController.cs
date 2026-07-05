using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GCI_Admin.Models;
using GCI_Admin.DBOperations;
using System.Threading.Tasks;

namespace GCI_Admin.Controllers
{
    public class RegisterController : Controller
    {
        private readonly AppDbContext _context;

        public RegisterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Register/Event/{eventId}")]
        public async Task<IActionResult> Event(int eventId)
        {
            var targetEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == eventId && e.IsActive == true && e.IsPaid);

            if (targetEvent == null)
            {
                return NotFound("Active paid event not found or has concluded.");
            }

            ViewBag.Event = targetEvent;
            ViewBag.Assemblies = await _context.Assemblies.ToListAsync();

            return View();
        }
    }
}
