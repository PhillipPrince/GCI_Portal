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
    }
}