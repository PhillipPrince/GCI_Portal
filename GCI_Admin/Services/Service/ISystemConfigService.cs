using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services
{
    public interface ISystemConfigService
    {
        Task<DbResponse<List<SystemConfig>>> GetAllConfigsAsync();
        Task<DbResponse<SystemConfig>> GetConfigByKeyAsync(string key);
        Task<DbResponse<SystemConfig>> CreateConfigAsync(SystemConfigDto dto);
        Task<DbResponse<SystemConfig>> UpdateConfigAsync(SystemConfigDto dto);
        Task<DbResponse<bool>> DeleteConfigAsync(int id);
    }
}