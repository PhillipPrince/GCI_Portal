namespace GCI_Admin.Models.DTOs
{
    public class LoginDto
    {
        public string EmailOrPhone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class ResetPasswordDto
    {
        public string EmailOrPhone { get; set; }
        public string OTPCode { get; set; }
        public string NewPassword { get; set; }
    }
    public class PasswordResetRequestDto
    {
        public string EmailOrPhone { get; set; }
    }
    public class ResendOtpDto
    {
        public string EmailOrPhone { get; set; }
    }

   
    public class LoginResponseData
    {
        public bool UseOtp { get; set; }
        public int UserRoleId { get; set; }
        public string UserRole { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string RedirectUrl { get; set; }
        public string Token { get; set; }
        public string ProfileImage { get; set; }
        public bool IsAuthenticated { get; set; }
        public int? UserId { get; set; }  // Make sure this exists
    }
}
