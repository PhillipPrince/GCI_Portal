using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GCI_Admin.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _userService;
        public AuthController(IAuthService userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var result = await _userService.ValidateUser(login);

            if (!result.IsSuccess)
                return Unauthorized(result);

            return Ok(result);
        }

        public async Task<IActionResult> ConfirmOtp([FromBody] ConfirmOtpDto confirmOtpDto)
        {
            var result = await _userService.ConfirmOtp(confirmOtpDto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto resendOtpDto)
        {
            var result = await _userService.ResendOtp(resendOtpDto);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.EmailOrPhone))
                return BadRequest(new { IsSuccess = false, Message = "Email or Phone is required." });

            var result = await _userService.RequestPasswordReset(request.EmailOrPhone);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        

        // Reset password using OTP
        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto reset)
        {
            if (reset == null || string.IsNullOrWhiteSpace(reset.EmailOrPhone)
                || string.IsNullOrWhiteSpace(reset.OTPCode) || string.IsNullOrWhiteSpace(reset.NewPassword))
                return BadRequest(new { IsSuccess = false, Message = "All fields are required." });

            var result = await _userService.ResetPassword(reset);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        public IActionResult Unauthorized()
        {
            HttpContext.Session.Clear();


            return View();
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();

            await HttpContext.SignOutAsync();

            HttpContext.Session.Remove("Token");

            return RedirectToAction("Index", "Auth");
        }
    }
}
