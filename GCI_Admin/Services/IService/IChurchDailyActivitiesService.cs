using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IChurchDailyActivitiesService
    {
        Task<ApiResponse<List<ChurchDailyActivity>>> GetAllAsync();
        Task<ApiResponse<ChurchDailyActivity>> GetByIdAsync(int id);
        Task<ApiResponse<ChurchDailyActivity>> CreateAsync(ChurchDailyActivityDto dto);
        Task<ApiResponse<ChurchDailyActivity>> UpdateAsync(int id, ChurchDailyActivityDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> ToggleStatusAsync(int id, bool isActive);
    }
}
