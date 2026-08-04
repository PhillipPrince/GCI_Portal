using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface ICollectionsService
    {
        Task<ApiResponse<List<Collection>>> GetAllAsync();
        Task<ApiResponse<List<Collection>>> GetByMemberIdAsync(int memberId);
        Task<ApiResponse<List<AccountReferenceSummaryDto>>> GetAccountReferenceSummaryAsync();
        Task<ApiResponse> VerifyCollection(VerifyCollectionRequest request);
        Task<ApiResponse<List<MeetingAttendance>>> GetActiveMeetingsAsync();
        Task<ApiResponse<List<Collection>>> GetFilteredCollectionsAsync(
            string search,
            string accountReference,
            string dateRange,
            string PaymentStatus,
            DateTime? fromDate,
            DateTime? toDate,
            int? filterYear,
            int? filterMonth,
            string paybill);
        Task<ApiResponse> SaveManualCollectionAsync(Collection collection);
        Task<ApiResponse> SendOtpAsync(SendOtpRequest request);
        Task<ApiResponse<List<object>>> GetActiveMembersDtoAsync();
        Task<ApiResponse<List<Collection>>> GetGBICollectionsAsync();
        Task<ApiResponse<List<AccountReferenceSummaryDto>>> GetGBIAccountReferenceSummaryAsync();
        Task<ApiResponse<List<Collection>>> GetChurchCollectionsAsync();
        Task<ApiResponse<List<AccountReferenceSummaryDto>>> GetChurchAccountReferenceSummaryAsync();
    }
}