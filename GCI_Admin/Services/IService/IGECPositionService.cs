using GCI_Admin.Models;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IGECPositionService
    {
        Task<ApiResponse<List<GECPosition>>> GetAllPositionsAsync();
        Task<ApiResponse<GECPosition>> GetPositionByIdAsync(int id);
        Task<ApiResponse<GECPosition>> CreatePositionAsync(GECPosition position);
        Task<ApiResponse<GECPosition>> UpdatePositionAsync(GECPosition position);
        Task<ApiResponse<bool>> DeletePositionAsync(int id);
        Task<ApiResponse<bool>> TogglePositionStatusAsync(int id, bool isActive);
    }
}
