using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IPaymentsService
    {
        Task<ApiResponse<List<Payment>>> GetAllAsync();

        Task<ApiResponse<List<Payment>>> GetByMemberIdAsync(int memberId);
        Task<ApiResponse<List<AccountReferenceSummaryDto>>> GetAccountReferenceSummaryAsync();
    }
}