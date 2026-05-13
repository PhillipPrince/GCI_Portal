using System;
using System.Linq;
using System.Threading.Tasks;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class AuthRepository
    {
        private readonly AppDbContext _context;
        private readonly Security _security = new Security();
        private readonly CommunicationService _communicationService;


        public AuthRepository(AppDbContext context, CommunicationService communicationService)
        {
            _context = context;
            _communicationService = communicationService;
        }

        // Create User
     //get user by id
        public async Task<DbResponse<Member>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _context.Members.FindAsync(id);
                if (user == null)
                {
                    return new DbResponse<Member>
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }
                return new DbResponse<Member>
                {
                    Success = true,
                    Message = "User retrieved successfully.",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UserRepository->GetUserByIdAsync->" + ex.Message);
                return new DbResponse<Member>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the user."
                };
            }
        }
        public async Task<DbResponse<Member>> ValidateUserAsync(LoginDto login)
        {
            var response = new DbResponse<Member>();

            try
            {
                if (string.IsNullOrEmpty(login.EmailOrPhone) || string.IsNullOrEmpty(login.Password))
                {
                    response.Success = false;
                    response.Message = "Email/Phone and Password are required.";
                    response.Data = null;
                    return response;
                }





                var user = await _context.Members
                    .FirstOrDefaultAsync(u => u.Email == login.EmailOrPhone || u.Phone == login.EmailOrPhone);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    response.Data = null;
                    return response;
                }

                var allowedRoles = new[] { 1, 5,6 }; 

                if (!allowedRoles.Contains(user.UserRole))
                {
                    response.Success = false;
                    response.Message = "Access denied. Only Admins and Pastors can perform this action.";
                    response.Data = null;
                    return response;
                }

                string decryptedPassword;
                try
                {
                    decryptedPassword = await _security.DecryptStringAES(user.PasswordHash, "GCI");
                }
                catch
                {
                    Loggers.DoLogs("Failed to decrypt password for user: " + login.EmailOrPhone);
                    response.Success = false;
                    response.Message = "Error validating user password.";
                    response.Data = null;
                    return response;
                }

                if (decryptedPassword == login.Password)
                {
                    response.Success = true;
                    response.Message = "User validated successfully.";
                    response.Data = user;
                    return response;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Invalid password.";
                    response.Data = null;
                    return response;
                }
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UserRepository->ValidateUserAsync->" + ex.Message);
                response.Success = false;
                response.Message = "An error occurred while validating the user.";
                response.Data = null;
                return response;
            }
        }
        public async Task<DbResponse<OTP>> GenerateAndInsertOtpAsync(string emailOrPhone, int expiryMinutes = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailOrPhone))
                {
                    return new DbResponse<OTP>
                    {
                        Success = false,
                        Message = "Email or Phone is required."
                    };
                }

                string otpCode = OTPGenerator.GenerateOTP();

                var otp = new OTP
                {
                    EmailOrPhone = emailOrPhone,
                    OTPCode = otpCode,
                    IsConfirmed = false,
                    CreatedAt = DateTime.Now,
                    ExpiresAt = DateTime.Now.AddMinutes(expiryMinutes)
                };

                var otpRecord = await _context.OTPs
                    .Where(o => o.EmailOrPhone == emailOrPhone)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();
                string userMessage = string.Empty;

                string otpText = MessageTemplates.GenerateOtpMessage("GCI", otpCode, expiryMinutes);

                if (OTPGenerator.IsPhoneNumber(emailOrPhone))
                {
                    try
                    {
                        var smsResult = await _communicationService.SendSmsAsync(emailOrPhone, otpText);

                        userMessage = "Your OTP code has been sent successfully!";
                    }
                    catch (Exception ex)
                    {
                        userMessage = "We were unable to send your OTP at this time. Please try again.";
                        Loggers.DoLogs($"SMS sending failed: {ex.Message}");
                    }
                }
                else if (OTPGenerator.IsEmail(emailOrPhone))
                {
                    try
                    {
                        var emailResult = await _communicationService.SendEmailAsync(emailOrPhone, "Your OTP Code", otpText);
                        userMessage = "Your OTP code has been sent successfully via email!";
                    }
                    catch (Exception ex)
                    {
                        userMessage = "We were unable to send your OTP email at this time. Please try again.";
                        Loggers.DoLogs($"Email sending failed: {ex.Message}");
                    }
                }
                else
                {
                    userMessage = "Invalid phone number or email address provided.";
                }





                if (otpRecord != null)
                {
                    _context.OTPs.Update(otpRecord);
                    otpRecord.OTPCode = otp.OTPCode;
                    otpRecord.IsConfirmed = otp.IsConfirmed;
                    otpRecord.CreatedAt = otp.CreatedAt;
                    otpRecord.ExpiresAt = otp.ExpiresAt;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.OTPs.Add(otp);
                    await _context.SaveChangesAsync();
                }



                return new DbResponse<OTP>
                {
                    Success = true,
                    Message = userMessage,
                    Data = otp
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UserRepository->GenerateAndInsertOtpAsync->" + ex.Message);
                return new DbResponse<OTP>
                {
                    Success = false,
                    Message = "An error occurred while generating OTP."
                };
            }
        }


        public async Task<DbResponse<OTP>> ConfirmOrRegenerateOtpAsync(ConfirmOtpDto confirm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(confirm.EmailOrPhone) || string.IsNullOrWhiteSpace(confirm.OTPCode))
                {
                    return new DbResponse<OTP>
                    {
                        Success = false,
                        Message = "Email/Phone and OTP code are required."
                    };
                }

                // Find the OTP record for this contact
                var otp = await _context.OTPs
                    .FirstOrDefaultAsync(o => o.EmailOrPhone == confirm.EmailOrPhone && o.OTPCode == confirm.OTPCode);

                if (otp == null || otp.IsConfirmed || otp.ExpiresAt < DateTime.Now)
                {
                    //var newOtpResult = await GenerateAndInsertOtpAsync(emailOrPhone, expiryMinutes);

                    return new DbResponse<OTP>
                    {
                        Success = false,
                        Message = "OTP was invalid, expired, or already used. A new OTP has been generated.",
                        Data = null
                    };
                }

                // Mark as confirmed
                otp.IsConfirmed = true;
                await _context.SaveChangesAsync();

                return new DbResponse<OTP>
                {
                    Success = true,
                    Message = "OTP confirmed successfully.",
                    Data = otp
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UserRepository->ConfirmOrRegenerateOtpAsync->" + ex.Message);
                return new DbResponse<OTP>
                {
                    Success = false,
                    Message = "An error occurred while confirming OTP."
                };
            }
        }

   
        public async Task<DbResponse<OTP>> RequestPasswordResetAsync(string emailOrPhone)
        {
            var response = new DbResponse<OTP>();

            try
            {
                if (string.IsNullOrWhiteSpace(emailOrPhone))
                {
                    response.Success = false;
                    response.Message = "Email or Phone is required.";
                    return response;
                }

                var user = await _context.Members
                    .FirstOrDefaultAsync(u => u.Email == emailOrPhone || u.Phone == emailOrPhone);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "No user found with the provided Email or Phone.";
                    return response;
                }

                var otpResult = await GenerateAndInsertOtpAsync(emailOrPhone);
                if (!otpResult.Success)
                {
                    return otpResult;
                }

                user.MustChangePassword = true;
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Password reset OTP generated successfully.";
                response.Data = otpResult.Data;
                return response;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UserRepository->RequestPasswordResetAsync->" + ex.Message);
                return new DbResponse<OTP>
                {
                    Success = false,
                    Message = "An error occurred while requesting password reset."
                };
            }
        }
        public async Task<DbResponse<Member>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var response = new DbResponse<Member>();

            try
            {
                if (string.IsNullOrWhiteSpace(dto.EmailOrPhone) ||
                    string.IsNullOrWhiteSpace(dto.OTPCode) ||
                    string.IsNullOrWhiteSpace(dto.NewPassword))
                {
                    response.Success = false;
                    response.Message = "Email/Phone, OTP, and new password are required.";
                    return response;
                }

                // Verify OTP
                var otpRecord = await _context.OTPs
                    .Where(o => o.EmailOrPhone == dto.EmailOrPhone &&
                                o.OTPCode == dto.OTPCode &&
                                !o.IsConfirmed &&
                                o.ExpiresAt > DateTime.Now)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (otpRecord == null)
                {
                    response.Success = false;
                    response.Message = "Invalid or expired OTP.";
                    return response;
                }

                // Mark OTP as confirmed
                otpRecord.IsConfirmed = true;

                // Find user
                var user = await _context.Members
                    .FirstOrDefaultAsync(u => u.Email == dto.EmailOrPhone || u.Phone == dto.EmailOrPhone);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "User not found.";
                    return response;
                }

                // Update password
                user.PasswordHash = _security.EncryptStringAES(dto.NewPassword, "GCI");
                user.MustChangePassword = false;
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Password reset successfully.";
                response.Data = user;
                return response;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UserRepository->ResetPasswordAsync->" + ex.Message);
                response.Success = false;
                response.Message = "An error occurred while resetting password.";
                return response;
            }
        }
        // Resend OTP
        public async Task<DbResponse<OTP>> ResendOtpAsync(ResendOtpDto dto, int expiryMinutes = 10)
        {
            var response = new DbResponse<OTP>();

            try
            {
                if (string.IsNullOrWhiteSpace(dto.EmailOrPhone))
                {
                    response.Success = false;
                    response.Message = "Email or Phone is required.";
                    return response;
                }

                // Generate a new OTP and insert/update
                var otpResult = await GenerateAndInsertOtpAsync(dto.EmailOrPhone, expiryMinutes);

                response.Success = otpResult.Success;
                response.Message = otpResult.Message;
                response.Data = otpResult.Data;

                return response;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UserRepository->ResendOtpAsync->" + ex.Message);
                response.Success = false;
                response.Message = "An error occurred while resending OTP.";
                return response;
            }
        }
        public async Task<DbResponse<Member>> GetUserByEmailOrPhoneAsync(string emailOrPhone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailOrPhone))
                {
                    return new DbResponse<Member>
                    {
                        Success = false,
                        Message = "Email or Phone is required."
                    };
                }

                var user = await _context.Members
                    .FirstOrDefaultAsync(u => u.Email == emailOrPhone || u.Phone == emailOrPhone);

                if (user == null)
                {
                    return new DbResponse<Member>
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                return new DbResponse<Member>
                {
                    Success = true,
                    Message = "User retrieved successfully.",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UserRepository->GetUserByEmailOrPhoneAsync->" + ex.Message);
                return new DbResponse<Member>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the user."
                };
            }
        }
        }
    }
