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
    public class CollectionsService : ICollectionsService
    {
        private readonly CollectionsRepository _repo;
        private readonly AppDbContext _appDbContext;
        private readonly AuthRepository _auth;
        private readonly IMembersService _membersService;

        public CollectionsService(CollectionsRepository repo, AppDbContext context, AuthRepository auth, IMembersService membersService)
        {
            _repo = repo;
            _appDbContext = context;
            _auth = auth;
            _membersService = membersService;
        }

        public async Task<ApiResponse<List<Collection>>> GetAllAsync()
        {
            var response = new ApiResponse<List<Collection>>();

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

        public async Task<ApiResponse<List<Collection>>> GetByMemberIdAsync(int memberId)
        {
            var response = new ApiResponse<List<Collection>>();

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

        public async Task<ApiResponse<List<MeetingAttendance>>> GetActiveMeetingsAsync()
        {
            var response = new ApiResponse<List<MeetingAttendance>>();
            try
            {
                var result = await _repo.GetActiveMeetingsAsync();
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

        public async Task<ApiResponse<List<Collection>>> GetFilteredCollectionsAsync(
            string search,
            string accountReference,
            string dateRange,
            string PaymentStatus,
            DateTime? fromDate,
            DateTime? toDate,
            int? filterYear,
            int? filterMonth,
            string paybill)
        {
            var response = new ApiResponse<List<Collection>>();
            try
            {
                var allCollections = await _repo.GetAllAsync();
                var Collections = allCollections?.Data ?? new List<Collection>();

                var query = Collections.AsQueryable();

                if (filterYear.HasValue)
                {
                    query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value.Year == filterYear.Value);
                }

                if (filterMonth.HasValue)
                {
                    query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value.Month == filterMonth.Value);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p =>
                        (p.PhoneNumber != null && p.PhoneNumber.Contains(search)) ||
                        (p.MpesaReceiptNumber != null && p.MpesaReceiptNumber.Contains(search)) ||
                        (p.AccountReference != null && p.AccountReference.Contains(search))
                    );
                }

                if (!string.IsNullOrEmpty(accountReference))
                {
                    query = query.Where(p => p.AccountReference == accountReference);
                }

                if (!string.IsNullOrEmpty(paybill))
                {
                    query = query.Where(p => p.Paybill == paybill);
                }

                if (!string.IsNullOrEmpty(PaymentStatus) && int.TryParse(PaymentStatus, out int statusId))
                {
                    query = query.Where(p => p.PaymentStatusId == statusId);
                }

                var now = DateTime.Now;
                switch (dateRange)
                {
                    case "today":
                        query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value.Date == now.Date);
                        break;
                    case "yesterday":
                        var yesterday = now.AddDays(-1).Date;
                        query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value.Date == yesterday);
                        break;
                    case "thisweek":
                        var weekStart = now.AddDays(-(int)now.DayOfWeek).Date;
                        query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value >= weekStart);
                        break;
                    case "thismonth":
                        var monthStart = new DateTime(now.Year, now.Month, 1);
                        query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value >= monthStart);
                        break;
                    case "lastmonth":
                        var lastMonth = now.AddMonths(-1);
                        var lastMonthStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
                        var lastMonthEnd = lastMonthStart.AddMonths(1).AddDays(-1);
                        query = query.Where(p => p.TransactionDate.HasValue &&
                                                p.TransactionDate.Value >= lastMonthStart &&
                                                p.TransactionDate.Value <= lastMonthEnd);
                        break;
                    case "thisyear":
                        var yearStart = new DateTime(now.Year, 1, 1);
                        query = query.Where(p => p.TransactionDate.HasValue && p.TransactionDate.Value >= yearStart);
                        break;
                    case "custom":
                        if (fromDate.HasValue && toDate.HasValue)
                        {
                            var toDateEnd = toDate.Value.AddDays(1).AddSeconds(-1);
                            query = query.Where(p => p.TransactionDate.HasValue &&
                                                    p.TransactionDate.Value >= fromDate.Value &&
                                                    p.TransactionDate.Value <= toDateEnd);
                        }
                        break;
                }

                var filteredCollections = query.OrderBy(p => p.Id).ToList();

                response.IsSuccess = true;
                response.Data = filteredCollections;
                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse> SaveManualCollectionAsync(Collection Collection)
        {
            var response = new ApiResponse();
            try
            {
                if (Collection == null || Collection.Amount <= 0)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Invalid Collection data";
                    return response;
                }

                var result = await _repo.SaveManualCollectionWithReconciliationAsync(Collection);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Code = result.Success ? "200" : "500";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse> SendOtpAsync(SendOtpRequest request)
        {
            var response = new ApiResponse();
            try
            {
                if (request != null && request.MeetingId > 0)
                {
                    var limitCheck = await _repo.CheckAndUpdateResendOtpLimitAsync(request.MeetingId);
                    if (!limitCheck.Success)
                    {
                        response.IsSuccess = false;
                        response.Code = "400";
                        response.Message = limitCheck.Message;
                        return response;
                    }
                }

                var otp = await _auth.GenerateAndInsertOtpAsync(request.EmailOrPhone, 10);
                if (otp != null)
                {
                    response.IsSuccess = true;
                    response.Code = "200";
                    response.Message = "OTP sent successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to send OTP";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<List<object>>> GetActiveMembersDtoAsync()
        {
            var response = new ApiResponse<List<object>>();
            try
            {
                var allMembersResponse = await _membersService.GetAllMembersAsync();
                var membersList = allMembersResponse?.Data ?? new List<Member>();

                var activeMembers = membersList
                    .Where(m => m.StatusId == 1)
                    .OrderBy(m => m.FirstName)
                    .Select(m => (object)new {
                        id = m.Id,
                        firstName = m.FirstName,
                        otherNames = m.OtherNames,
                        email = m.Email,
                        phone = m.Phone,
                        gender = m.Gender
                    })
                    .ToList();

                response.IsSuccess = true;
                response.Data = activeMembers;
                response.Code = "200";
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
                var meeting = _appDbContext.MeetingAttendances.FirstOrDefault(m => m.MeetingAttendancesId == request.MeetingId);

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

                Collection CreateCollection(string accountReference, decimal amount)
                {
                    return new Collection
                    {
                        MemberId = 0,
                        MeetingId = request.MeetingId,
                        AccountReference = accountReference,
                        MerchantRequestID = "N/A",
                        CheckoutRequestID = "N/A",
                        MpesaReceiptNumber = "N/A",
                        PhoneNumber = "N/A",
                        TransactionDate = meeting != null ? meeting.MeetingDate : now,
                        ResultCode = 0,
                        ResultDesc = "Cash Collection",
                        Amount = amount,
                        PaymentStatusId = 2,
                        CreatedAt = now
                    };
                }

                var CollectionDefinitions = new[]
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

                var Collections = CollectionDefinitions
                    .Where(x => x.Amount > 0)
                    .Select(x => CreateCollection(x.Name, x.Amount))
                    .ToList();

                if (Collections.Any())
                {
                    await _appDbContext.Collections.AddRangeAsync(Collections);
                    await _appDbContext.SaveChangesAsync();

                    Loggers.EventLogs(
                        $"Created {Collections.Count} Collection records for Meeting ID {request.MeetingId}");
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

        public async Task<ApiResponse<List<Collection>>> GetGBICollectionsAsync()
        {
            var response = new ApiResponse<List<Collection>>();
            try
            {
                var result = await _repo.GetGBICollectionsAsync();
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

        public async Task<ApiResponse<List<AccountReferenceSummaryDto>>> GetGBIAccountReferenceSummaryAsync()
        {
            var response = new ApiResponse<List<AccountReferenceSummaryDto>>();
            try
            {
                var result = await _repo.GetGBIAccountReferenceSummaryAsync();
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

        public async Task<ApiResponse<List<Collection>>> GetChurchCollectionsAsync()
        {
            var response = new ApiResponse<List<Collection>>();
            try
            {
                var result = await _repo.GetChurchCollectionsAsync();
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

        public async Task<ApiResponse<List<AccountReferenceSummaryDto>>> GetChurchAccountReferenceSummaryAsync()
        {
            var response = new ApiResponse<List<AccountReferenceSummaryDto>>();
            try
            {
                var result = await _repo.GetChurchAccountReferenceSummaryAsync();
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
    }
}
