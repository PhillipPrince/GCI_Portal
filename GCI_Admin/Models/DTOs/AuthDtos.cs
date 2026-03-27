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
}
