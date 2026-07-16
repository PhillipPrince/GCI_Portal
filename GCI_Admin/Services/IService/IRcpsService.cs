using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IRcpsService
    {
        Task<ApiResponse<Rcps>> CreateRcpsAsync(RcpsDto dto);

        Task<ApiResponse<List<Rcps>>> GetAllRcpsAsync();

        Task<ApiResponse<Rcps>> GetRcpsByIdAsync(int id);

        Task<ApiResponse<Rcps>> UpdateRcpsAsync(Rcps dto);

        Task<ApiResponse<bool>> DeleteRcpsAsync(int id);

        Task<ApiResponse<RcpsPledges>> CreateRcpsPledgeAsync(RcpsPledgesDto dto);

        Task<ApiResponse<List<RcpsPledges>>> GetAllRcpsPledgesAsync();

        Task<ApiResponse<RcpsPledges>> GetRcpsPledgeByIdAsync(int id);

        Task<ApiResponse<RcpsPledges>> UpdateRcpsPledgeAsync(int id, RcpsPledgesDto dto);

        Task<ApiResponse<bool>> DeleteRcpsPledgeAsync(int id);
        Task<ApiResponse<List<RcpsPledges>>> GetPledgesByRcpsIdAsync(int id);
        Task<ApiResponse<RcpsCountyCoordinator>> CreateRcpsCountyCoordinatorAsync(RcpsCountyCoordinatorDto dto);
        Task<ApiResponse<List<RcpsCountyCoordinator>>> GetAllRcpsCountyCoordinatorsAsync();
        Task<ApiResponse<RcpsCountyCoordinator>> GetRcpsCountyCoordinatorByIdAsync(int id);
        Task<ApiResponse<List<RcpsCountyCoordinator>>> GetRcpsCountyCoordinatorsByRcpsAsync(int rcpsId);
        Task<ApiResponse<RcpsCountyCoordinator>> UpdateRcpsCountyCoordinatorAsync(RcpsCountyCoordinatorDto dto);
        Task<ApiResponse<bool>> DeleteRcpsCountyCoordinatorAsync(int id);
        Task<ApiResponse<bool>> ToggleCountyCoordinatorStatusAsync(int id, bool isActive);
        Task<ApiResponse<List<County>>> GetAllCountiesAsync();
    }
}