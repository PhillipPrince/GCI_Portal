using System.Collections.Generic;
using System.Threading.Tasks;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface ITitlePrefixService
    {
        Task<ApiResponse<List<TitlePrefix>>> GetAllPrefixesAsync();
        Task<ApiResponse<List<TitlePrefix>>> GetActivePrefixesAsync();
        Task<ApiResponse<TitlePrefix>> GetPrefixByIdAsync(int id);
        Task<ApiResponse<TitlePrefix>> CreatePrefixAsync(TitlePrefixDto dto);
        Task<ApiResponse<TitlePrefix>> UpdatePrefixAsync(TitlePrefixDto dto);
        Task<ApiResponse<bool>> DeletePrefixAsync(int id);
        Task<ApiResponse<bool>> ToggleStatusAsync(int id, bool isActive);
    }
}
