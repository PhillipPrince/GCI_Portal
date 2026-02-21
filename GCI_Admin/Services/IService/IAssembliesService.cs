using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IAssembliesService
    {
        Task<ApiResponse<Assembly>> CreateAssemblyAsync(AssemblyDto dto);

        Task<ApiResponse<List<Assembly>>> GetAllAssembliesAsync();
        Task<ApiResponse<Assembly>> GetAssemblyByIdAsync(int assemblyId);

        Task<ApiResponse<Assembly>> UpdateAssemblyAsync(int assemblyId, AssemblyDto dto);

        Task<ApiResponse<bool>> DeleteAssemblyAsync(int assemblyId);

        Task<ApiResponse<bool>> ToggleAssemblyStatusAsync(int assemblyId, bool isActive);
        Task<ApiResponse<List<AssemblyLeader>>> GetAllAssemblyLeadersAsync();
    }
}
