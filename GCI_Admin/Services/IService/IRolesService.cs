using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IRolesService
    {
        Task<ApiResponse<Role>> CreateRoleAsync(RoleDto dto);
        Task<ApiResponse<List<Role>>> GetAllRolesAsync();
        Task<ApiResponse<Role>> GetRoleByIdAsync(int roleId);
        Task<ApiResponse<Role>> UpdateRoleAsync(int roleId, RoleDto dto);
        Task<ApiResponse<bool>> DeleteRoleAsync(int roleId);
    }
}