using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface ILeadershipService
    {
        Task<ApiResponse<Deacon>> CreateDeaconAsync(DeaconDto dto);
        Task<ApiResponse<List<Deacon>>> GetAllDeaconsAsync();
        Task<ApiResponse<Deacon>> GetDeaconByIdAsync(int id);
        Task<ApiResponse<Deacon>> UpdateDeaconAsync(int id, DeaconDto dto);
        Task<ApiResponse<bool>> DeleteDeaconAsync(int id);
        Task<ApiResponse<bool>> ToggleDutyStatusAsync(int id, bool onDuty);
    }
}