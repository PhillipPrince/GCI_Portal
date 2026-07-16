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

        [HttpGet("Register/CheckRegistration")]
        public async Task<IActionResult> CheckRegistration(string phone, int eventId)
        {
            if (string.IsNullOrWhiteSpace(phone) || eventId <= 0)
                return BadRequest("Invalid input");

            phone = global::Utils.PhoneHelper.NormalizeKenyanPhoneOrEmail(phone);

            var registrationsQuery = await _context.EventRegistrations
                .Include(r => r.Member)
                .Where(r => r.EventId == eventId && 
                            (r.GuestPhone == phone || (r.Member != null && r.Member.Phone == phone)))
                .OrderByDescending(r => r.RegistrationDate)
                .ToListAsync();

            if (!registrationsQuery.Any())
            {
                return Json(new { isRegistered = false });
            }

            var records = registrationsQuery.Select(r => new {
                paymentStatusId = r.PaymentStatusId,
                registrationId = r.RegistrationId,
                guestName = r.GuestName ?? (r.Member != null ? $"{r.Member.FirstName} {r.Member.OtherNames}".Trim() : "N/A")
            }).ToList();

            return Json(new { 
                isRegistered = true, 
                records = records 
            });
        }

        [HttpGet("Register/CheckPaymentStatus/{registrationId}")]
        public async Task<IActionResult> CheckPaymentStatus(int registrationId)
        {
            var registration = await _context.EventRegistrations
                .FirstOrDefaultAsync(r => r.RegistrationId == registrationId);

            if (registration == null)
                return NotFound();

            return Json(new { paymentStatusId = registration.PaymentStatusId });
        }

        [HttpGet("Register/Usher/{eventId}")]
        public async Task<IActionResult> Usher(int eventId)
        {
            var targetEvent = await _context.Events
                .FirstOrDefaultAsync(e => e.EventId == eventId && e.IsActive == true);

            if (targetEvent == null)
            {
                return NotFound("Active event not found or has concluded.");
            }

            ViewBag.Event = targetEvent;
            ViewBag.Assemblies = await _context.Assemblies.ToListAsync();

            return View("Usher");
        }

        [HttpPost("Register/UsherSubmit")]
        public async Task<IActionResult> UsherSubmit([FromBody] UsherRegistrationDto dto)
        {
            if (dto == null || dto.eventId <= 0)
                return BadRequest(new { isSuccess = false, message = "Invalid data." });

            var eventItem = await _context.Events.FirstOrDefaultAsync(e => e.EventId == dto.eventId);
            if (eventItem == null)
                return NotFound(new { isSuccess = false, message = "Event not found." });

            dto.guestPhone = global::Utils.PhoneHelper.NormalizeKenyanPhoneOrEmail(dto.guestPhone);

            var memberId = 0;
            var existingMember = await _context.Members.FirstOrDefaultAsync(m => m.Phone == dto.guestPhone || (!string.IsNullOrEmpty(dto.guestEmail) && m.Email == dto.guestEmail));
            if (existingMember != null)
            {
                memberId = existingMember.Id;
            }

            EventRegistration existingRegistration = null;
            if (memberId != 0)
            {
                existingRegistration = await _context.EventRegistrations
                    .FirstOrDefaultAsync(r => r.EventId == dto.eventId && r.MemberId == memberId);
            }
            var existingGuestRegistration = await _context.EventRegistrations
                .FirstOrDefaultAsync(r => r.EventId == dto.eventId && r.GuestPhone == dto.guestPhone && r.GuestName == dto.guestName);

            int newPaymentStatusId = dto.isPaid ? 4 : 2; // 4 = Paid, 2 = Pending/Not Paid
            if ((existingRegistration != null && existingRegistration.PaymentStatusId == 4) || 
                (existingGuestRegistration != null && existingGuestRegistration.PaymentStatusId == 4))
            {
                return BadRequest(new { isSuccess = false, message = "Guest is already registered and paid." });
            }
            else if ((existingRegistration != null && existingRegistration.PaymentStatusId != 4) || 
                     (existingGuestRegistration != null && existingGuestRegistration.PaymentStatusId != 4))
            {
                var regToUpdate = existingRegistration ?? existingGuestRegistration;
                regToUpdate.PaymentStatusId = newPaymentStatusId;
                regToUpdate.RegistrationDate = System.DateTime.UtcNow;
                regToUpdate.AmountPaid = dto.amountPaid;
                await _context.SaveChangesAsync();
                
                return Ok(new { isSuccess = true, message = dto.isPaid ? "Registration updated to paid." : "Registration updated." });
            }

            var registration = new EventRegistration
            {
                EventId = dto.eventId,
                MemberId = memberId,
                GuestName = dto.guestName,
                GuestEmail = dto.guestEmail,
                GuestPhone = dto.guestPhone,
                GuestAssembly = dto.guestAssembly,
                GuestAgeGroup = dto.guestAgeGroup,
                PaymentStatusId = newPaymentStatusId,
                AmountPaid = dto.amountPaid,
                RegistrationDate = System.DateTime.UtcNow,
                HasAttended = false
            };

            _context.EventRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            return Ok(new { isSuccess = true, message = "Registration successful." });
        }
    }

    public class UsherRegistrationDto
    {
        public int eventId { get; set; }
        public string guestName { get; set; }
        public string guestPhone { get; set; }
        public string guestEmail { get; set; }
        public string guestAssembly { get; set; }
        public string guestAgeGroup { get; set; }
        public decimal amountPaid { get; set; }
        public bool isPaid { get; set; }
    }
}
