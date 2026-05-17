using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GCI_Admin.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
          
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                var result = await _userService.ValidateUser(login);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning($"Login failed for {login.EmailOrPhone}: {result.Message}");
                    return Unauthorized(new { isSuccess = false, message = result.Message });
                }

                // If OTP is required, don't create session yet
                if (result.Data.UseOtp)
                {
                    return Ok(new
                    {
                        isSuccess = true,
                        message = result.Message,
                        data = result.Data
                    });
                }

                // Create cookie session for authenticated user
                await CreateUserSession(result.Data);

                return Ok(new
                {
                    isSuccess = true,
                    message = result.Message,
                    data = result.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Login error for {login.EmailOrPhone}");
                return StatusCode(500, new { isSuccess = false, message = "An error occurred during login" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOtp([FromBody] ConfirmOtpDto confirmOtpDto)
        {
            try
            {
                var result = await _userService.ConfirmOtp(confirmOtpDto);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning($"OTP confirmation failed for {confirmOtpDto.EmailOrPhone}: {result.Message}");
                    return BadRequest(new { isSuccess = false, message = result.Message });
                }

                // Create cookie session after successful OTP verification
                await CreateUserSession(result.Data);

                return Ok(new
                {
                    isSuccess = true,
                    message = result.Message,
                    data = result.Data
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"OTP confirmation error for {confirmOtpDto.EmailOrPhone}");
                return StatusCode(500, new { isSuccess = false, message = "An error occurred during OTP verification" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto resendOtpDto)
        {
            try
            {
                var result = await _userService.ResendOtp(resendOtpDto);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { isSuccess = false, message = result.Message });
                }

                return Ok(new { isSuccess = true, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Resend OTP error for {resendOtpDto.EmailOrPhone}");
                return StatusCode(500, new { isSuccess = false, message = "An error occurred while resending OTP" });
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.EmailOrPhone))
                    return BadRequest(new { isSuccess = false, message = "Email or Phone is required." });

                var result = await _userService.RequestPasswordReset(request.EmailOrPhone);

                if (!result.IsSuccess)
                    return BadRequest(new { isSuccess = false, message = result.Message });

                return Ok(new { isSuccess = true, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Password reset request error for {request?.EmailOrPhone}");
                return StatusCode(500, new { isSuccess = false, message = "An error occurred while requesting password reset" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto reset)
        {
            try
            {
                if (reset == null || string.IsNullOrWhiteSpace(reset.EmailOrPhone)
                    || string.IsNullOrWhiteSpace(reset.OTPCode) || string.IsNullOrWhiteSpace(reset.NewPassword))
                    return BadRequest(new { isSuccess = false, message = "All fields are required." });

                var result = await _userService.ResetPassword(reset);

                if (!result.IsSuccess)
                    return BadRequest(new { isSuccess = false, message = result.Message });

                return Ok(new { isSuccess = true, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Password reset error for {reset?.EmailOrPhone}");
                return StatusCode(500, new { isSuccess = false, message = "An error occurred while resetting password" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Unauthorized()
        {
            // Clear session without redirecting
            await ClearUserSession();
            return View("Unauthorized");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await ClearUserSession();
                _logger.LogInformation("User logged out successfully");
                return Ok(new { isSuccess = true, message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return BadRequest(new { isSuccess = false, message = "Error during logout" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            try
            {
                await ClearUserSession();
                _logger.LogInformation("User logged out successfully");
                return RedirectToAction("Index", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return RedirectToAction("Index", "Auth");
            }
        }

        // Helper method to clear user session and cookies
        private async Task ClearUserSession()
        {
            try
            {
                // Remove JWT token cookie
                Response.Cookies.Delete("GCI_Token");

                // Clear session
                HttpContext.Session.Clear();

                // Sign out from cookie authentication
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Remove token from session
                HttpContext.Session.Remove("Token");

                // Clear all authentication-related cookies
                Response.Cookies.Delete(".AspNetCore.Session");
                Response.Cookies.Delete("GCI_Auth_Cookie");
                Response.Cookies.Delete(".AspNetCore.Cookies");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing user session");
            }
        }

        // Helper method to create user session with cookie authentication
        private async Task CreateUserSession(LoginResponseData userData)
        {
            if (userData == null) return;

            // Store JWT token in cookie for API calls
            if (!string.IsNullOrEmpty(userData.Token))
            {
                Response.Cookies.Append("GCI_Token", userData.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddHours(8)
                });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userData.UserId?.ToString() ?? ""),
                new Claim(ClaimTypes.Name, userData.UserName ?? ""),
                new Claim(ClaimTypes.Email, userData.Email ?? ""),
                new Claim(ClaimTypes.Role, userData.UserRoleId.ToString()),
                new Claim("RoleId", userData.UserRoleId.ToString()),
                new Claim("UserRole", userData.UserRole ?? ""),
                new Claim("UserId", userData.UserId?.ToString() ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        // Get current user info
        [HttpGet]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized(new { isSuccess = false, message = "Not authenticated" });

            var userInfo = new
            {
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Name = User.Identity.Name,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
                RoleId = User.FindFirst("RoleId")?.Value,
                UserRole = User.FindFirst("UserRole")?.Value
            };

            return Ok(new { isSuccess = true, data = userInfo });
        }

        // Check if user is authenticated
        [HttpGet]
        public IActionResult CheckAuthentication()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok(new
                {
                    isAuthenticated = true,
                    userName = User.Identity.Name,
                    role = User.FindFirst(ClaimTypes.Role)?.Value,
                    roleId = User.FindFirst("RoleId")?.Value
                });
            }

            return Ok(new { isAuthenticated = false });
        }
    }
}