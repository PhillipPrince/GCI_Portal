using System.ComponentModel.DataAnnotations;

namespace GCI_Admin.Models.DTOs
{
    public class ConfirmOtpDto
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Email or Phone")]
        public string EmailOrPhone { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        [Display(Name = "OTP Code")]
        public string OTPCode { get; set; } = string.Empty;
    }
}
