using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.DBOperations.Repositories;
using Utils;
using GCI_Admin.Services.IService;

namespace GCI_Admin.Services
{
    public class SystemConfigService : ISystemConfigService
    {
        private readonly SystemConfigRepository _repository;

        public SystemConfigService(SystemConfigRepository repository)
        {
            _repository = repository;
        }

        public async Task<DbResponse<List<SystemConfig>>> GetAllConfigsAsync()
        {
            try
            {
                return await _repository.GetAllConfigsAsync();
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("SystemConfigService->GetAllConfigsAsync->" + ex);
                return new DbResponse<List<SystemConfig>>
                {
                    Success = false,
                    Message = "Failed to retrieve configs"
                };
            }
        }

        public async Task<DbResponse<SystemConfig>> GetConfigByKeyAsync(string key)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return new DbResponse<SystemConfig>
                    {
                        Success = false,
                        Message = "Config key is required"
                    };
                }

                return await _repository.GetConfigByKeyAsync(key.Trim());
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("SystemConfigService->GetConfigByKeyAsync->" + ex);
                return new DbResponse<SystemConfig>
                {
                    Success = false,
                    Message = "Failed to retrieve config"
                };
            }
        }

        public async Task<DbResponse<SystemConfig>> CreateConfigAsync(SystemConfigDto dto)
        {
            try
            {
                // Basic validation
                if (dto == null)
                {
                    return new DbResponse<SystemConfig>
                    {
                        Success = false,
                        Message = "Invalid request"
                    };
                }

                if (string.IsNullOrWhiteSpace(dto.ConfigKey))
                {
                    return new DbResponse<SystemConfig>
                    {
                        Success = false,
                        Message = "Config key is required"
                    };
                }

                if (string.IsNullOrWhiteSpace(dto.ConfigValue))
                {
                    return new DbResponse<SystemConfig>
                    {
                        Success = false,
                        Message = "Config value is required"
                    };
                }

                dto.ConfigKey = dto.ConfigKey.Trim();

                return await _repository.CreateConfigAsync(dto);
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("SystemConfigService->CreateConfigAsync->" + ex);
                return new DbResponse<SystemConfig>
                {
                    Success = false,
                    Message = "Failed to create config"
                };
            }
        }

        public async Task<DbResponse<SystemConfig>> UpdateConfigAsync(SystemConfigDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return new DbResponse<SystemConfig>
                    {
                        Success = false,
                        Message = "Invalid request"
                    };
                }

                if (string.IsNullOrWhiteSpace(dto.ConfigKey))
                {
                    return new DbResponse<SystemConfig>
                    {
                        Success = false,
                        Message = "Config key is required"
                    };
                }

                dto.ConfigKey = dto.ConfigKey.Trim();

                return await _repository.UpdateConfigByKeyAsync(dto);
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("SystemConfigService->UpdateConfigAsync->" + ex);
                return new DbResponse<SystemConfig>
                {
                    Success = false,
                    Message = "Failed to update config"
                };
            }
        }

        public async Task<DbResponse<bool>> DeleteConfigAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Invalid config ID"
                    };
                }

                return await _repository.DeleteConfigAsync(id);
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("SystemConfigService->DeleteConfigAsync->" + ex);
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = "Failed to delete config"
                };
            }
        }
    }
}