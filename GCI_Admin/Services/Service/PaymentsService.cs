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
                var confirmOtpDto = new ConfirmOtpDto
                {
                    OTPCode = request.OtpCode,
                    EmailOrPhone = request.EmailOrPhone
                };

    var user = await _appDbContext.Members
        .FirstOrDefaultAsync(m =>
            m.Phone == request.EmailOrPhone ||
            m.Email == request.EmailOrPhone);

                if (user == null)
                {
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        Code = "404",
                        Message = "Member not found."
                    };
                }

                // Use phone for OTP validation
                confirmOtpDto.EmailOrPhone = user.Phone;

                var confirm = await _auth.ConfirmOrRegenerateOtpAsync(confirmOtpDto);

                if (confirm?.Data?.IsConfirmed != true)
                {
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        Code = "400",
                        Message = "Invalid or expired OTP."
                    };
                }

                var collectionVerified = await _appDbContext.ServiceCollectionSummaries
                    .FirstOrDefaultAsync(s =>
                        s.MeetingAttendancesId == request.MeetingId);

                if (collectionVerified == null)
                {
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        Code = "404",
                        Message = "Collection record not found."
                    };
                }

                if (collectionVerified.IsVerified)
                {
                    return new ApiResponse
                    {
                        IsSuccess = true,
                        Code = "200",
                        Message = "Collection has already been verified."
                    };
                }

                collectionVerified.IsVerified = true;
                collectionVerified.VerifiedBy = user.Id;
                collectionVerified.VerifiedAt = DateTime.Now;

                await _appDbContext.SaveChangesAsync();

                Loggers.EventLogs(
                    $"Collection for Meeting ID {request.MeetingId} verified by Member ID {user.Id}");

                var now = DateTime.Now;

                Payment CreatePayment(string accountReference, decimal amount)
                {
                    return new Payment
                    {
                        MemberId = 0,
                        AccountReference = accountReference,
                        MerchantRequestID = "N/A",
                        CheckoutRequestID = "N/A",
                        MpesaReceiptNumber = "N/A",
                        PhoneNumber = "N/A",
                        TransactionDate = now,
                        ResultCode = 0,
                        ResultDesc = "Cash Collection",
                        Amount = amount,
                        PaymentStatusId = 2,
                        CreatedAt = now
                    };
                }

                var paymentDefinitions = new[]
                {
        new { Name = "Tithes", Amount = collectionVerified.Tithes },
        new { Name = "Offerings", Amount = collectionVerified.Offerings },
        new { Name = "Sunday School", Amount = collectionVerified.SundaySchool },
        new { Name = "Thanksgiving", Amount = collectionVerified.Thanksgiving },
        new { Name = "Missions", Amount = collectionVerified.Missions },
        new { Name = "Projects", Amount = collectionVerified.Projects },
        new { Name = "Youth", Amount = collectionVerified.Youth },
        new { Name = "Widows & Orphans", Amount = collectionVerified.WidowsOrphans },
        new { Name = "Others", Amount = collectionVerified.Others }
    };

                var payments = paymentDefinitions
                    .Where(x => x.Amount > 0)
                    .Select(x => CreatePayment(x.Name, x.Amount))
                    .ToList();

                if (payments.Any())
                {
                    await _appDbContext.Payments.AddRangeAsync(payments);
                    await _appDbContext.SaveChangesAsync();

                    Loggers.EventLogs(
                        $"Created {payments.Count} payment records for Meeting ID {request.MeetingId}");
                }

                return new ApiResponse
                {
                    IsSuccess = true,
                    Code = "200",
                    Message = "Collection verified successfully."
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"VerifyCollection Error: {ex}");

                return new ApiResponse
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.InnerException?.Message ?? "An error occurred while verifying the collection."
                };
            }


}

    }
}
