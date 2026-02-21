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

        // ✅ Toggle active/inactive status
        Task<ApiResponse<bool>> ToggleMinistryStatusAsync(int ministryId, bool isActive);

        // ✅ Ministry Leaders
        Task<ApiResponse<List<MinistryLeader>>> GetAllMinistryLeadersAsync();
    }
}