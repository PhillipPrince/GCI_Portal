using ClosedXML.Excel;
using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;
using GCI_Admin.Utils;

namespace GCI_Admin.Services.Service
{
    public class MinistriesService : IMinistriesService
    {
        private readonly MinistriesRepository _ministriesRepository;
        private readonly AppDbContext _context;

        public MinistriesService(MinistriesRepository ministriesRepository, AppDbContext context)
        {
            _ministriesRepository = ministriesRepository;
            _context = context;
        }

        // ✅ CREATE MINISTRY
        public async Task<ApiResponse<Ministry>> CreateMinistryAsync(MinistryDto dto)
        {
            var response = new ApiResponse<Ministry>();

            try
            {
                var result = await _ministriesRepository.CreateMinistryAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to create ministry";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Ministry created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET ALL MINISTRIES
        public async Task<ApiResponse<List<Ministry>>> GetAllMinistriesAsync()
        {
            var response = new ApiResponse<List<Ministry>>();

            try
            {
                var result = await _ministriesRepository.GetAllMinistriesAsync();

                response.Data = result.Data;
                response.Message = "Ministries retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET MINISTRY BY ID
        public async Task<ApiResponse<Ministry>> GetMinistryByIdAsync(int ministryId)
        {
            var response = new ApiResponse<Ministry>();

            try
            {
                var result = await _ministriesRepository.GetMinistryByIdAsync(ministryId);

                if (result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Ministry not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Ministry retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ UPDATE MINISTRY
        public async Task<ApiResponse<Ministry>> UpdateMinistryAsync(int ministryId, MinistryDto dto)
        {
            var response = new ApiResponse<Ministry>();

            try
            {
                var result = await _ministriesRepository.UpdateMinistryAsync(ministryId, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Ministry not found or update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Ministry updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ DELETE MINISTRY (soft-delete)
        public async Task<ApiResponse<bool>> DeleteMinistryAsync(int ministryId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _ministriesRepository.DeleteMinistryAsync(ministryId);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Ministry not found or delete failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = result.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ TOGGLE ACTIVE STATUS
        public async Task<ApiResponse<bool>> ToggleMinistryStatusAsync(int ministryId, bool isActive)
        {
            var ministry = await _context.Ministries.FindAsync(ministryId);

            if (ministry == null)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "404",
                    Message = "Ministry not found"
                };
            }

            ministry.IsActive = isActive;
            ministry.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                IsSuccess = true,
                Code = "200",
                Message = isActive ? "Ministry activated successfully." : "Ministry deactivated successfully.",
                Data = true
            };
        }

        // ✅ GET ALL MINISTRY LEADERS
        public async Task<ApiResponse<List<MinistryLeader>>> GetAllMinistryLeadersAsync()
        {
            var response = new ApiResponse<List<MinistryLeader>>();

            try
            {
                var result = await _ministriesRepository.GetMinistryLeadersAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to retrieve ministry leaders";
                    return response;
                }

                var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                foreach (var leader in result.Data)
                {
                    if (leader.Member != null)
                    {
                        leader.Member.ProfileImage = ImageHelper.ReadImage(imageFolder, leader.MemberId.ToString());
                    }
                }

                response.Data = result.Data;
                response.Message = "Ministry leaders retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ CREATE MINISTRY LEADER
        public async Task<ApiResponse<MinistryLeader>> CreateMinistryLeaderAsync(MinistryLeaderDto dto)
        {
            var response = new ApiResponse<MinistryLeader>();

            try
            {
                var result = await _ministriesRepository.CreateMinistryLeaderAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                if (!string.IsNullOrEmpty(dto.ProfileImageBase64))
                {
                    string saved = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ProfileImageBase64), imageFolder, $"{result.Data.MemberId}", "jpg");
                    Loggers.EventLogs($"Saved profile image for ministry leader {result.Data.MinistryLeaderId} at {saved}");
                }

                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in CreateMinistryLeaderAsync service: {ex}");
            }

            return response;
        }

        // ✅ GET MINISTRY LEADER BY ID
        public async Task<ApiResponse<MinistryLeader>> GetMinistryLeaderByIdAsync(int ministryLeaderId)
        {
            var response = new ApiResponse<MinistryLeader>();

            try
            {
                var result = await _ministriesRepository.GetMinistryLeaderByIdAsync(ministryLeaderId);

                if (!result.Success || result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Ministry leader not found";
                    return response;
                }

                var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                if (result.Data.Member != null)
                {
                    result.Data.Member.ProfileImage = ImageHelper.ReadImage(imageFolder, result.Data.MemberId.ToString());
                }

                response.Data = result.Data;
                response.Message = "Ministry leader retrieved successfully";
                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in GetMinistryLeaderByIdAsync service: {ex}");
            }

            return response;
        }

        // ✅ UPDATE MINISTRY LEADER
        public async Task<ApiResponse<MinistryLeader>> UpdateMinistryLeaderAsync(int ministryLeaderId, MinistryLeaderDto dto)
        {
            var response = new ApiResponse<MinistryLeader>();

            try
            {
                var result = await _ministriesRepository.UpdateMinistryLeaderAsync(ministryLeaderId, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                if (!string.IsNullOrEmpty(dto.ProfileImageBase64))
                {
                    string saved = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ProfileImageBase64), imageFolder, $"{result.Data.MemberId}", "jpg");
                    Loggers.EventLogs($"Saved profile image for ministry leader {result.Data.MinistryLeaderId} on update at {saved}");
                }

                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in UpdateMinistryLeaderAsync service: {ex}");
            }

            return response;
        }

        // ✅ DELETE MINISTRY LEADER
        public async Task<ApiResponse<bool>> DeleteMinistryLeaderAsync(int ministryLeaderId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _ministriesRepository.DeleteMinistryLeaderAsync(ministryLeaderId);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in DeleteMinistryLeaderAsync service: {ex}");
            }

            return response;
        }

        // ✅ GET MINISTRY LEADERS BY MINISTRY
        public async Task<ApiResponse<List<MinistryLeader>>> GetMinistryLeadersByMinistryAsync(int ministryId)
        {
            var response = new ApiResponse<List<MinistryLeader>>();

            try
            {
                var result = await _ministriesRepository.GetMinistryLeadersByMinistryAsync(ministryId);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                foreach (var leader in result.Data)
                {
                    if (leader.Member != null)
                    {
                        leader.Member.ProfileImage = ImageHelper.ReadImage(imageFolder, leader.MemberId.ToString());
                    }
                }

                response.Data = result.Data;
                response.Message = "Ministry leaders retrieved successfully";
                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in GetMinistryLeadersByMinistryAsync service: {ex}");
            }

            return response;
        }

        // ✅ GET ACTIVE MINISTRY LEADERS
        public async Task<ApiResponse<List<MinistryLeader>>> GetActiveMinistryLeadersAsync()
        {
            var response = new ApiResponse<List<MinistryLeader>>();

            try
            {
                var result = await _ministriesRepository.GetActiveMinistryLeadersAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                foreach (var leader in result.Data)
                {
                    if (leader.Member != null)
                    {
                        leader.Member.ProfileImage = ImageHelper.ReadImage(imageFolder, leader.MemberId.ToString());
                    }
                }

                response.Data = result.Data;
                response.Message = "Active ministry leaders retrieved successfully";
                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in GetActiveMinistryLeadersAsync service: {ex}");
            }

            return response;
        }

        // ✅ TOGGLE STATUS
        public async Task<ApiResponse<bool>> ToggleMinistryLeaderStatusAsync(int ministryLeaderId, bool isActive)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _ministriesRepository.ToggleMinistryLeaderStatusAsync(ministryLeaderId, isActive);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in ToggleMinistryLeaderStatusAsync service: {ex}");
            }

            return response;
        }
    }
}