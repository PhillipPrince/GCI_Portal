using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IMinistriesService
    {
        // ✅ Ministry CRUD
        Task<ApiResponse<Ministry>> CreateMinistryAsync(MinistryDto dto);
        Task<ApiResponse<List<Ministry>>> GetAllMinistriesAsync();
        Task<ApiResponse<Ministry>> GetMinistryByIdAsync(int ministryId);
        Task<ApiResponse<Ministry>> UpdateMinistryAsync(int ministryId, MinistryDto dto);
        Task<ApiResponse<bool>> DeleteMinistryAsync(int ministryId);
        Task<ApiResponse<bool>> ToggleMinistryStatusAsync(int ministryId, bool isActive);

        // ✅ Ministry Leader CRUD
        Task<ApiResponse<MinistryLeader>> CreateMinistryLeaderAsync(MinistryLeaderDto dto);
        Task<ApiResponse<List<MinistryLeader>>> GetAllMinistryLeadersAsync();
        Task<ApiResponse<MinistryLeader>> GetMinistryLeaderByIdAsync(int ministryLeaderId);
        Task<ApiResponse<MinistryLeader>> UpdateMinistryLeaderAsync(int ministryLeaderId, MinistryLeaderDto dto);
        Task<ApiResponse<bool>> DeleteMinistryLeaderAsync(int ministryLeaderId);
        Task<ApiResponse<List<MinistryLeader>>> GetMinistryLeadersByMinistryAsync(int ministryId);
        Task<ApiResponse<List<MinistryLeader>>> GetActiveMinistryLeadersAsync();
        Task<ApiResponse<bool>> ToggleMinistryLeaderStatusAsync(int ministryLeaderId, bool isActive);
    }
}