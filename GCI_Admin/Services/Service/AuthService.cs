
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly AuthRepository _userRepository;
        private readonly JwtTokenService _jwtTokenService;
        private readonly SystemConfigRepository _sys;
        private readonly string _imageBasePath = "";
        private readonly SessionManager _sessionManager;
        private readonly DevelopmentSettings _devSettings;

        public AuthService(AuthRepository userRepository, JwtTokenService jwtTokenService, SystemConfigRepository sys, SessionManager session, IOptions<DevelopmentSettings> devSettings)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _sys = sys;
            _imageBasePath = SystemConfigHelper.GetImageBasePathAsync(_sys).GetAwaiter().GetResult();
            _devSettings = devSettings.Value;
            _sessionManager = session;

        }

       
        public async Task<ApiResponse<Member>> ValidateUser(LoginDto login)
        {
            var response = new ApiResponse<Member>();

            try
            {
                var dbResponse = await _userRepository.ValidateUserAsync(login);

                if (!dbResponse.Success || dbResponse.Data == null)
                {
                    response.IsSuccess = false;
                    response.Message = dbResponse.Message ?? "Invalid credentials";
                    response.Code = "401";
                    response.Data = null;

                    Loggers.DoLogs($"Login failed for {login.EmailOrPhone}: {response.Message}");
                    return response;
                }

                var user = dbResponse.Data;

                var permissions = PermissionHelper.GetPermissions(user.UserRole);

                var token = _jwtTokenService.GenerateToken(
                    user.FirstName + " " + user.OtherNames,
                    user.Email,
                    user.UserRole,
                    permissions
                );

                var profileImage = ImageHelper.ReadImage(_imageBasePath, user.Id.ToString());


                var otp = await _sys.GetConfigByKeyAsync("USE_OTP");


                if(otp.Data.ConfigValue == "true" && !_devSettings.IsDev)
                {
                    var otpExp=await _sys.GetConfigByKeyAsync("OTP_EXPIRY_MINUTES");
                    string phone = user.Phone;

                    await _userRepository.GenerateAndInsertOtpAsync(phone, int.Parse(otpExp.Data.ConfigValue));
                    user.UseOtp = true;
                }
                else
                {
                    _sessionManager.SetUserSession(user);

                }



                user.Token = token;
                user.ProfileImage = profileImage;

                response.IsSuccess = true;
                response.Message = "Login successful";
                response.Code = "200";
                response.Data = user;

                Loggers.DoLogs($"Login successful for {login.EmailOrPhone}");
                return response;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"ValidateUser Exception for {login.EmailOrPhone}: {ex.Message}");
                response.IsSuccess = false;
                response.Message = "An error occurred while validating the user.";
                response.Code = "500";
                response.Data = null;
                return response;
            }
        }
        public async Task<ApiResponse<OTP>> ConfirmOtp(ConfirmOtpDto confirm)
        {
            var response = new ApiResponse<OTP>();

            try
            {
                var dbResponse = await _userRepository.ConfirmOrRegenerateOtpAsync(confirm);

                if (dbResponse.Success)
                {
                    var userResponse = await _userRepository.GetUserByEmailOrPhoneAsync(confirm.EmailOrPhone);

                    if (userResponse.Success && userResponse.Data != null)
                    {
                        _sessionManager.SetUserSession(userResponse.Data);
                    }
                }

                response.IsSuccess = dbResponse.Success;
                response.Message = dbResponse.Message;
                response.Code = dbResponse.Success ? "200" : "400";
                response.Data = dbResponse.Data;

                return response;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"ConfirmOtp Exception for {confirm.EmailOrPhone}: {ex.Message}");
                response.IsSuccess = false;
                response.Message = "An error occurred while confirming the OTP.";
                response.Code = "500";
                response.Data = null;
                return response;
            }
        }

        public async Task<ApiResponse<OTP>> RequestPasswordReset(string emailOrPhone)
        {
            var response = new ApiResponse<OTP>();

            try
            {
                var dbResponse = await _userRepository.RequestPasswordResetAsync(emailOrPhone);

                response.IsSuccess = dbResponse.Success;
                response.Message = dbResponse.Message;
                response.Code = dbResponse.Success ? "200" : "400";
                response.Data = dbResponse.Data;

                Loggers.DoLogs($"Password reset request for {emailOrPhone}: {dbResponse.Message}");
                return response;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"RequestPasswordReset Exception for {emailOrPhone}: {ex.Message}");
                response.IsSuccess = false;
                response.Message = "An error occurred while requesting password reset.";
                response.Code = "500";
                response.Data = null;
                return response;
            }
        }

        // Reset password using OTP
        public async Task<ApiResponse<Member>> ResetPassword(ResetPasswordDto dto)
        {
            var response = new ApiResponse<Member>();

            try
            {
                var dbResponse = await _userRepository.ResetPasswordAsync(dto);

                response.IsSuccess = dbResponse.Success;
                response.Message = dbResponse.Message;
                response.Code = dbResponse.Success ? "200" : "400";
                response.Data = dbResponse.Data;

                Loggers.DoLogs($"Password reset attempt for {dto.EmailOrPhone}: {dbResponse.Message}");
                return response;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"ResetPassword Exception for {dto.EmailOrPhone}: {ex.Message}");
                response.IsSuccess = false;
                response.Message = "An error occurred while resetting the password.";
                response.Code = "500";
                response.Data = null;
                return response;
            }
        }
        public async Task<ApiResponse<OTP>> ResendOtp(ResendOtpDto resendOtpDto)
        {
            var response = new ApiResponse<OTP>();
            try
            {
                var dbResponse = await _userRepository.ResendOtpAsync(resendOtpDto);
                response.IsSuccess = dbResponse.Success;
                response.Message = dbResponse.Message;
                response.Code = dbResponse.Success ? "200" : "400";
                response.Data = dbResponse.Data;
                Loggers.DoLogs($"Resend OTP attempt for {resendOtpDto.EmailOrPhone}: {dbResponse.Message}");
                return response;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"ResendOtp Exception for {resendOtpDto.EmailOrPhone}: {ex.Message}");
                response.IsSuccess = false;
                response.Message = "An error occurred while resending the OTP.";
                response.Code = "500";
                response.Data = null;
                return response;
            }
        }
        }
}
