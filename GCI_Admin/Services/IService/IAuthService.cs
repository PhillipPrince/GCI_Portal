
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IAuthService
    {
        Task<ApiResponse<Member>> ValidateUser(LoginDto login);
        Task<ApiResponse<OTP>> ConfirmOtp(ConfirmOtpDto confirm);
        Task<ApiResponse<Member>> ResetPassword(ResetPasswordDto dto);
        Task<ApiResponse<OTP>> RequestPasswordReset(string emailOrPhone);
        Task<ApiResponse<OTP>> ResendOtp(ResendOtpDto resendOtpDto);
    }
}
