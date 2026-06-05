using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repo_GCI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class PaymentsService : IPaymentsService
    {
        private readonly PaymentsRepository _repo;
        private readonly AppDbContext _appDbContext;
        private readonly AuthRepository _auth;

        public PaymentsService(PaymentsRepository repo,AppDbContext context, AuthRepository auth)
        {
            _repo = repo;
            _appDbContext = context;
            _auth = auth;
        }

        public async Task<ApiResponse<List<Payment>>> GetAllAsync()
        {
            var response = new ApiResponse<List<Payment>>();

            try
            {
                var result = await _repo.GetAllAsync();

                response.IsSuccess = result.Success;
                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = result.Success ? "200" : "400";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<List<Payment>>> GetByMemberIdAsync(int memberId)
        {
            var response = new ApiResponse<List<Payment>>();

            try
            {
                var result = await _repo.GetByMemberIdAsync(memberId);

                response.IsSuccess = result.Success;
                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = result.Success ? "200" : "400";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<List<AccountReferenceSummaryDto>>> GetAccountReferenceSummaryAsync()
        {
            var response = new ApiResponse<List<AccountReferenceSummaryDto>>();

            try
            {
                var result = await _repo.GetAccountReferenceSummaryAsync();

                response.IsSuccess = result.Success;
                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = result.Success ? "200" : "400";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse> VerifyCollection(VerifyCollectionRequest request)
        {
            try
            {
                ConfirmOtpDto confirmOtpDto = new ConfirmOtpDto
                {
                    OTPCode = request.OtpCode,
                    EmailOrPhone = request.EmailOrPhone
                };

                var user = _appDbContext.Members.Where(m => m.Phone == request.EmailOrPhone || m.Email == request.EmailOrPhone).FirstOrDefault();
                if (user == null)
                {
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        Code = "404",
                        Message = "Member not found."
                    };
                }

                var confirm = await _auth.ConfirmOrRegenerateOtpAsync(confirmOtpDto);

                if (confirm != null && confirm.Data.IsConfirmed)
                {
                    var collectionVerified = await _appDbContext.ServiceCollectionSummaries
                        .FirstOrDefaultAsync(s => s.MeetingAttendancesId == request.MeetingId);

                    if (collectionVerified == null)
                    {
                        return new ApiResponse
                        {
                            IsSuccess = false,
                            Code = "404",
                            Message = "Collection record not found."
                        };
                    }
                    
                    if (!collectionVerified.IsVerified)
                    {
                        collectionVerified.IsVerified = true;
                        collectionVerified.VerifiedBy = user.Id;
                        collectionVerified.VerifiedAt = DateTime.Now;
                        await _appDbContext.SaveChangesAsync();
                        Loggers.EventLogs($"Collection for Meeting ID {request.MeetingId} verified by Member ID {user.Id}");
                    }

                    if (collectionVerified != null)
                    {
                        var payments = new List<Payment>();

                        // Tithes
                        if (collectionVerified.Tithes > 0)
                        {
                            payments.Add(new Payment
                            {
                                MemberId = 0,
                                AccountReference = "Tithes",
                                MerchantRequestID = "N/A",
                                CheckoutRequestID = "N/A",
                                MpesaReceiptNumber = "N/A",
                                PhoneNumber = "N/A",
                                TransactionDate = DateTime.Now,
                                ResultDesc = "0",
                                Amount = collectionVerified.Tithes,
                                PaymentStatusId = 2,
                                CreatedAt = DateTime.Now
                            });
                        }

                        // Offerings
                        if (collectionVerified.Offerings > 0)
                        {
                            payments.Add(new Payment
                            {
                                MemberId = 0,
                                AccountReference = "Offerings",
                                MerchantRequestID = "N/A",
                                CheckoutRequestID = "N/A",
                                MpesaReceiptNumber = "N/A",
                                PhoneNumber = "N/A",
                                TransactionDate = DateTime.Now,

                                ResultDesc = "0",
                                Amount = collectionVerified.Offerings,
                                PaymentStatusId = 2,
                                CreatedAt = DateTime.Now
                            });
                        }

                        // Sunday School
                        if (collectionVerified.SundaySchool > 0)
                        {
                            payments.Add(new Payment
                            {
                                MemberId = 0,
                                AccountReference = "Sunday School",
                                MerchantRequestID = "N/A",
                                CheckoutRequestID = "N/A",
                                MpesaReceiptNumber = "N/A",
                                PhoneNumber = "N/A",
                                TransactionDate = DateTime.Now,

                                ResultDesc = "0",
                                Amount = collectionVerified.SundaySchool,
                                PaymentStatusId = 2,
                                CreatedAt = DateTime.Now
                            });
                        }

                        // Thanksgiving
                        if (collectionVerified.Thanksgiving > 0)
                        {
                            payments.Add(new Payment
                            {
                                MemberId = 0,
                                AccountReference = "Thanksgiving",
                                MerchantRequestID = "N/A",
                                CheckoutRequestID = "N/A",
                                MpesaReceiptNumber = "N/A",
                                PhoneNumber = "N/A",
                                TransactionDate = DateTime.Now,

                                ResultDesc = "0",
                                Amount = collectionVerified.Thanksgiving,
                                PaymentStatusId = 2,
                                CreatedAt = DateTime.Now
                            });
                        }

                        // Missions
                        if (collectionVerified.Missions > 0)
                        {
                            payments.Add(new Payment
                            {
                                MemberId = 0,
                                AccountReference = "Missions",
                                MerchantRequestID = "N/A",
                                CheckoutRequestID = "N/A",
                                MpesaReceiptNumber = "N/A",
                                PhoneNumber = "N/A",
                                TransactionDate = DateTime.Now,

                                ResultDesc = "0",
                                Amount = collectionVerified.Missions,
                                PaymentStatusId = 2,
                                CreatedAt = DateTime.Now
                            });
                        }

                        // Projects
                        if (collectionVerified.Projects > 0)
                        {
                            payments.Add(new Payment
                            {
                                MemberId = 0,
                                AccountReference = "Projects",
                                MerchantRequestID = "N/A",
                                CheckoutRequestID = "N/A",
                                MpesaReceiptNumber = "N/A",
                                PhoneNumber = "N/A",
                                TransactionDate = DateTime.Now,

                                ResultDesc = "0",
                                Amount = collectionVerified.Projects,
                                PaymentStatusId = 2,
                                CreatedAt = DateTime.Now
                            });
                        }

                        // Youth
                        if (collectionVerified.Youth > 0)
                        {
                            payments.Add(new Payment
                            {
                                MemberId = 0,
                                AccountReference = "Youth",
                                MerchantRequestID = "N/A",
                                CheckoutRequestID = "N/A",
                                MpesaReceiptNumber = "N/A",
                                PhoneNumber = "N/A",
                                    TransactionDate= DateTime.Now,

                                ResultDesc = "0",
                                Amount = collectionVerified.Youth,
                                PaymentStatusId = 2,
                                CreatedAt = DateTime.Now
                            });
                        }

                        // Widows & Orphans
                        if (collectionVerified.WidowsOrphans > 0)
                        {
                            payments.Add(new Payment
                            {
                                MemberId = 0,
                                AccountReference = "Widows & Orphans",
                                MerchantRequestID = "N/A",
                                CheckoutRequestID = "N/A",
                                MpesaReceiptNumber = "N/A",
                                PhoneNumber = "N/A",

                                TransactionDate = DateTime.Now,
                                ResultDesc = "0",
                                Amount = collectionVerified.WidowsOrphans,
                                PaymentStatusId = 2,
                                CreatedAt = DateTime.Now
                            });
                        }

                        // Others
                        if (collectionVerified.Others > 0)
                        {
                            payments.Add(new Payment
                            {
                                MemberId = 0,
                                AccountReference = "Others",
                                MerchantRequestID = "N/A",
                                CheckoutRequestID = "N/A",
                                MpesaReceiptNumber = "N/A",
                                PhoneNumber = "N/A",
                                TransactionDate = DateTime.Now,
                                ResultDesc = "0",
                                Amount = collectionVerified.Others,
                                PaymentStatusId = 2,
                                CreatedAt = DateTime.Now
                            });
                        }

                        if (payments.Any())
                        {
                            await _appDbContext.Payments.AddRangeAsync(payments);
                            await _appDbContext.SaveChangesAsync();
                        }
                    }


                    return new ApiResponse
                    {
                        IsSuccess = true,
                        Code = "200",
                        Message = "Collection verified successfully."
                    };
                }

                return new ApiResponse
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = "Invalid or expired OTP."
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"VerifyCollection Error: {ex}");

                return new ApiResponse
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = "An error occurred while verifying the collection."
                };
            }
        }

    }
}