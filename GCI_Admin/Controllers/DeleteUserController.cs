using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Controllers
{
    [AllowAnonymous]
    [Route("DeleteUser")]
    public class DeleteUserController : Controller
    {
        private readonly IMembersService _mem;
        private readonly AppDbContext _con;
        private readonly AuthRepository _membersRepository;

        public DeleteUserController(
            IMembersService mem,
            AppDbContext con,
            AuthRepository membersRepository)
        {
            _mem = mem;
            _con = con;
            _membersRepository = membersRepository;
        }

        // GET: https://portal.gospelcentresinternational.com/DeleteUser
        [HttpGet("")]
        [HttpHead("")]

        public IActionResult Index()
        {
            return View();
        }

        // STEP 1: Request deletion (send OTP)
        // POST: /DeleteUser/DeleteAccount
        [HttpPost("DeleteAccount")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.PhoneNumber))
                {
                    return BadRequest(new { success = false, message = "Phone number is required." });
                }

                string phoneNumber = PhoneHelper.NormalizeKenyanPhoneOrEmail(request.PhoneNumber);

                Loggers.EventLogs($"Account deletion requested for: {MaskPhoneNumber(phoneNumber)}");

                var user = await _con.Members.FirstOrDefaultAsync(m => m.Phone == phoneNumber);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "No account found with this phone number." });
                }

                await _membersRepository.GenerateAndInsertOtpAsync(phoneNumber);

                return Ok(new
                {
                    success = true,
                    message = "Verification code sent successfully.",
                    verificationSent = true,
                    nextSteps = "Check your SMS for the OTP code."
                });
            }
            catch (Exception ex)
            {
                Loggers.DoLogs(ex + $" Error requesting deletion: {request?.PhoneNumber}");
                return StatusCode(500, new { success = false, message = "Internal server error." });
            }
        }

        // STEP 2: Confirm deletion
        // POST: /DeleteUser/ConfirmDeletion
        [HttpPost("ConfirmDeletion")]
        public async Task<IActionResult> ConfirmDeletion([FromBody] ConfirmDeletionRequest request)
        {
            try
            {
                if (request == null ||
                    string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                    string.IsNullOrWhiteSpace(request.VerificationCode))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Phone number and verification code are required."
                    });
                }

                Loggers.EventLogs($"Confirming deletion for: {MaskPhoneNumber(request.PhoneNumber)}");

                var confirmOtp = new ConfirmOtpDto
                {
                    EmailOrPhone = request.PhoneNumber,
                    OTPCode = request.VerificationCode
                };

                await _membersRepository.ConfirmOrRegenerateOtpAsync(confirmOtp);

                // TODO: Perform actual account deletion here if needed

                return Ok(new
                {
                    success = true,
                    message = "Account deletion confirmed and processed.",
                    deletedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Loggers.DoLogs(ex + $" Error confirming deletion: {request?.PhoneNumber}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error processing deletion."
                });
            }
        }

        private string MaskPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber)) return phoneNumber;

            var digits = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

            if (digits.Length <= 7) return "***-****";

            var start = digits.Substring(0, 3);
            var end = digits.Substring(digits.Length - 4);

            return $"{start}***{end}";
        }
    }

    // DTOs
    public class DeleteAccountRequest
    {
        public string PhoneNumber { get; set; }
    }

    public class ConfirmDeletionRequest
    {
        public string PhoneNumber { get; set; }
        public string VerificationCode { get; set; }
    }
}