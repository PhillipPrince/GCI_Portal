using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface ILeadershipService
    {
        // ===================== DEACONS =====================
        Task<ApiResponse<Deacon>> CreateDeaconAsync(DeaconDto dto);
        Task<ApiResponse<List<Deacon>>> GetAllDeaconsAsync();
        Task<ApiResponse<Deacon>> GetDeaconByIdAsync(int id);
        Task<ApiResponse<Deacon>> UpdateDeaconAsync(int id, DeaconDto dto);
        Task<ApiResponse<bool>> DeleteDeaconAsync(int id);
        Task<ApiResponse<bool>> ToggleDutyStatusAsync(int id, bool onDuty);

        // ===================== ELDERS =====================
        Task<ApiResponse<Elder>> CreateElderAsync(ElderDto dto);
        Task<ApiResponse<List<Elder>>> GetAllEldersAsync();
        Task<ApiResponse<Elder>> GetElderByIdAsync(int id);
        Task<ApiResponse<Elder>> UpdateElderAsync(int id, ElderDto dto);
        Task<ApiResponse<bool>> DeleteElderAsync(int id);
    }
}