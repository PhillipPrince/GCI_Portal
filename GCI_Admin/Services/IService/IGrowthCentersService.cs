using Azure;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IGrowthCentersService
    {
        Task<ApiResponse<GrowthCenter>> CreateGrowthCenterAsync(GrowthCenterDto dto);
        Task<ApiResponse<List<GrowthCenter>>> GetAllGrowthCentersAsync();
        Task<ApiResponse<GrowthCenter>> GetGrowthCenterByIdAsync(int id);
        Task<ApiResponse<GrowthCenter>> UpdateGrowthCenterAsync(int id, GrowthCenterDto dto);
        Task<ApiResponse<bool>> DeleteGrowthCenterAsync(int id);
        Task<ApiResponse<bool>> ToggleGrowthCenterStatusAsync(int id, bool isActive);
        Task<ApiResponse<List<GrowthCenterLeader>>> GetAllGrowthCenterLeadersAsync();
        Task<ApiResponse<GrowthCenterLeader>> GetGCLeaderByIdAsync(int id);
        Task<ApiResponse<GrowthCenterLeader>> GetGCLeaderByMemberAndCenterAsync(int memberId, int centerId);
        Task<ApiResponse<GrowthCenterLeader>> CreateGCLeaderAsync(GCLeaderDto dto);
        Task<ApiResponse<GrowthCenterLeader>> UpdateGCLeaderAsync(GCLeaderDto dto);
        Task<ApiResponse<bool>> DeleteGCLeaderAsync(int id);
        Task<ApiResponse<bool>> ToggleGCLeaderStatusAsync(int id, bool isActive);
        Task<ApiResponse<List<GrowthCenterLeader>>> GetGrowthCenterLeadersByCenterAsync(int centerId);

        // ✅ Growth Center Members
        Task<ApiResponse<List<GrowthCenterMember>>> GetGrowthCenterMembersAsync(int centerId);
        Task<ApiResponse<bool>> AddMemberToGrowthCenterAsync(int centerId, int memberId);
    }
}