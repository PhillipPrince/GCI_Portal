using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class SystemConfigRepository
    {
        private readonly AppDbContext _context;

        public SystemConfigRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse<List<SystemConfig>>> GetAllConfigsAsync()
        {
            var response = new DbResponse<List<SystemConfig>>();

            try
            {
                response.Data = await _context.SystemConfig
                    .OrderBy(c => c.ConfigKey)
                    .ToListAsync();

                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("SystemConfigRepository->GetAllConfigsAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<DbResponse<SystemConfig>> GetConfigByKeyAsync(string key)
        {
            var response = new DbResponse<SystemConfig>();

            try
            {
                var config = await _context.SystemConfig
                    .FirstOrDefaultAsync(c => c.ConfigKey == key);

                if (config != null)
                {
                    response.Data = config;
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Message = "Config not found";
                    Loggers.DoLogs("Fetched Config: Not Found ----> " + key);
                }
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("SystemConfigRepository->GetConfigByKeyAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<DbResponse<SystemConfig>> CreateConfigAsync(SystemConfigDto dto)
        {
            var response = new DbResponse<SystemConfig>();

            try
            {
                var exists = await _context.SystemConfig
                    .AnyAsync(c => c.ConfigKey == dto.ConfigKey);

                if (exists)
                {
                    response.Success = false;
                    response.Message = "Config key already exists";
                    return response;
                }

                var entity = new SystemConfig
                {
                    ConfigKey = dto.ConfigKey,
                    ConfigValue = dto.ConfigValue,
                    Description = dto.Description,
                    IsEditable = dto.IsEditable,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now
                };

                _context.SystemConfig.Add(entity);
                await _context.SaveChangesAsync();

                response.Data = entity;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("SystemConfigRepository->CreateConfigAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<DbResponse<SystemConfig>> UpdateConfigByKeyAsync(SystemConfigDto dto)
        {
            var response = new DbResponse<SystemConfig>();

            try
            {
                var config = await _context.SystemConfig
                    .FirstOrDefaultAsync(c => c.ConfigKey == dto.ConfigKey);

                if (config == null)
                {
                    response.Success = false;
                    response.Message = $"Config with key '{dto.ConfigKey}' not found";
                    return response;
                }

                if (!config.IsEditable)
                {
                    response.Success = false;
                    response.Message = "This config is locked and cannot be edited";
                    return response;
                }

                config.ConfigValue = dto.ConfigValue;
                config.Description = dto.Description;
                config.IsEditable = dto.IsEditable;
                config.IsActive = dto.IsActive;
                config.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                response.Data = config;
                response.Success = true;
                response.Message = "Config updated successfully";
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("SystemConfigRepository->UpdateConfigByKeyAsync->" + ex.Message);
                response.Success = false;
                response.Message = "Error updating config";
            }

            return response;
        }

        public async Task<DbResponse<bool>> DeleteConfigAsync(int id)
        {
            var response = new DbResponse<bool>();

            try
            {
                var config = await _context.SystemConfig
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (config == null)
                {
                    response.Success = false;
                    response.Message = "Config not found";
                    return response;
                }

                _context.SystemConfig.Remove(config);
                await _context.SaveChangesAsync();

                response.Data = true;
                response.Success = true;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("SystemConfigRepository->DeleteConfigAsync->" + ex.Message);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
