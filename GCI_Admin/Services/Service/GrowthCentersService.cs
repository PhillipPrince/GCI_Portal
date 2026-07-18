using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GCI_Admin.Utils;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class GrowthCentersService : IGrowthCentersService
    {
        private readonly GrowthCentersRepository _repository;
        private readonly AppDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;

        public GrowthCentersService(GrowthCentersRepository repository, AppDbContext context, ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        // ✅ CREATE GROWTH CENTER
        public async Task<ApiResponse<GrowthCenter>> CreateGrowthCenterAsync(GrowthCenterDto dto)
        {
            var response = new ApiResponse<GrowthCenter>();

            try
            {
                var result = await _repository.CreateGrowthCenterAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to create growth center";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Growth center created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET ALL GROWTH CENTERS
        public async Task<ApiResponse<List<GrowthCenter>>> GetAllGrowthCentersAsync()
        {
            var response = new ApiResponse<List<GrowthCenter>>();

            try
            {
                var result = await _repository.GetAllGrowthCentersAsync();

                response.Data = result.Data;
                response.Message = "Growth centers retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET GROWTH CENTER BY ID
        public async Task<ApiResponse<GrowthCenter>> GetGrowthCenterByIdAsync(int id)
        {
            var response = new ApiResponse<GrowthCenter>();

            try
            {
                var result = await _repository.GetGrowthCenterByIdAsync(id);

                if (result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Growth center not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Growth center retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ UPDATE GROWTH CENTER
        public async Task<ApiResponse<GrowthCenter>> UpdateGrowthCenterAsync(int id, GrowthCenterDto dto)
        {
            var response = new ApiResponse<GrowthCenter>();

            try
            {
                var result = await _repository.UpdateGrowthCenterAsync(id, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Growth center not found or update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Growth center updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ DELETE GROWTH CENTER
        public async Task<ApiResponse<bool>> DeleteGrowthCenterAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _repository.DeleteGrowthCenterAsync(id);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Growth center not found or delete failed";
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
        public async Task<ApiResponse<bool>> ToggleGrowthCenterStatusAsync(int id, bool isActive)
        {
            var center = await _context.GrowthCenters.FindAsync(id);

            if (center == null)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "404",
                    Message = "Growth center not found"
                };
            }

            center.IsActive = isActive;
            center.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                IsSuccess = true,
                Code = "200",
                Message = isActive ? "Growth center activated successfully." : "Growth center deactivated successfully.",
                Data = true
            };
        }

        // ✅ GET ALL GROWTH CENTER LEADERS
        public async Task<ApiResponse<List<GrowthCenterLeader>>> GetAllGrowthCenterLeadersAsync()
        {
            var response = new ApiResponse<List<GrowthCenterLeader>>();

            try
            {
                var result = await _repository.GetGrowthCenterLeadersAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to retrieve growth center leaders";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Growth center leaders retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<GrowthCenterLeader>> GetGCLeaderByIdAsync(int id)
        {
            var response = new ApiResponse<GrowthCenterLeader>();
            try
            {
                var leader = await _context.GrowthCenterLeaders
                    .Include(l => l.Member)
                    .Include(l => l.GrowthCenter)
                    .FirstOrDefaultAsync(l => l.GrowthCenterLeaderId == id);
                if (leader == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "GC Leader not found";
                    return response;
                }
                var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                if (leader.Member != null)
                {
                    leader.Member.ProfileImage = ImageHelper.ReadImage(imageFolder, leader.MemberId.ToString());
                }
                response.Data = leader;
                response.Message = "Leader retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<GrowthCenterLeader>> GetGCLeaderByMemberAndCenterAsync(int memberId, int centerId)
        {
            var response = new ApiResponse<GrowthCenterLeader>();
            try
            {
                var leader = await _context.GrowthCenterLeaders
                    .FirstOrDefaultAsync(l => l.MemberId == memberId && l.GrowthCenterId == centerId && l.IsActive);
                if (leader == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Leader not found";
                    return response;
                }
                response.Data = leader;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<GrowthCenterLeader>> CreateGCLeaderAsync(GCLeaderDto dto)
        {
            var response = new ApiResponse<GrowthCenterLeader>();
            try
            {
                var leader = new GrowthCenterLeader
                {
                    MemberId = dto.MemberId,
                    GrowthCenterId = dto.GrowthCenterId,
                    Bio = dto.Bio,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now
                };
                _context.GrowthCenterLeaders.Add(leader);
                await _context.SaveChangesAsync();

                var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                if (!string.IsNullOrEmpty(dto.ProfileImageBase64))
                {
                    string saved = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ProfileImageBase64), imageFolder, $"{dto.MemberId}", "jpg");
                    Loggers.EventLogs($"Saved profile image for GC leader at {saved}");

                    try
                    {
                        var cloudinaryUrl = await _cloudinaryService.UploadBase64ImageAsync(dto.ProfileImageBase64);
                        if (!string.IsNullOrEmpty(cloudinaryUrl))
                        {
                            var memberToUpdate = await _context.Members.FindAsync(dto.MemberId);
                            if (memberToUpdate != null)
                            {
                                memberToUpdate.ProfilePictureUrl = cloudinaryUrl;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Loggers.DoLogs($"Cloudinary upload failed for GC leader member {dto.MemberId}: {ex}");
                    }
                }

                response.Data = leader;
                response.Message = "GC Leader assigned successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<GrowthCenterLeader>> UpdateGCLeaderAsync(GCLeaderDto dto)
        {
            var response = new ApiResponse<GrowthCenterLeader>();
            try
            {
                var leader = await _context.GrowthCenterLeaders.FindAsync(dto.GCLeaderId);
                if (leader == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "GC Leader not found";
                    return response;
                }
                leader.MemberId = dto.MemberId;
                leader.GrowthCenterId = dto.GrowthCenterId;
                leader.Bio = dto.Bio;
                leader.StartDate = dto.StartDate;
                leader.EndDate = dto.EndDate;
                leader.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                if (!string.IsNullOrEmpty(dto.ProfileImageBase64))
                {
                    string saved = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ProfileImageBase64), imageFolder, $"{dto.MemberId}", "jpg");
                    Loggers.EventLogs($"Saved profile image for GC leader on update at {saved}");

                    try
                    {
                        var cloudinaryUrl = await _cloudinaryService.UploadBase64ImageAsync(dto.ProfileImageBase64);
                        if (!string.IsNullOrEmpty(cloudinaryUrl))
                        {
                            var memberToUpdate = await _context.Members.FindAsync(dto.MemberId);
                            if (memberToUpdate != null)
                            {
                                memberToUpdate.ProfilePictureUrl = cloudinaryUrl;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Loggers.DoLogs($"Cloudinary upload failed for GC leader member {dto.MemberId}: {ex}");
                    }
                }

                response.Data = leader;
                response.Message = "GC Leader updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteGCLeaderAsync(int id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var leader = await _context.GrowthCenterLeaders.FindAsync(id);
                if (leader == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "GC Leader not found";
                    return response;
                }
                leader.IsActive = false;
                await _context.SaveChangesAsync();
                response.Data = true;
                response.Message = "GC Leader deleted successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> ToggleGCLeaderStatusAsync(int id, bool isActive)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var leader = await _context.GrowthCenterLeaders.FindAsync(id);
                if (leader == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "GC Leader not found";
                    return response;
                }
                leader.IsActive = isActive;
                await _context.SaveChangesAsync();
                response.Data = true;
                response.Message = isActive ? "GC Leader activated successfully" : "GC Leader deactivated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<List<GrowthCenterLeader>>> GetGrowthCenterLeadersByCenterAsync(int centerId)
        {
            var response = new ApiResponse<List<GrowthCenterLeader>>();
            try
            {
                var result = await _repository.GetGrowthCenterLeadersByCenterAsync(centerId);
                
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "GC leaders retrieved successfully";
                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in GetGrowthCenterLeadersByCenterAsync service: {ex}");
            }
            return response;
        }

        public async Task<ApiResponse<List<GrowthCenterMember>>> GetGrowthCenterMembersAsync(int centerId)
        {
            var response = new ApiResponse<List<GrowthCenterMember>>();
            try
            {
                var result = await _repository.GetGrowthCenterMembersAsync(centerId);
                
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "GC members retrieved successfully";
                response.Code = "200";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in GetGrowthCenterMembersAsync service: {ex}");
            }
            return response;
        }

        public async Task<ApiResponse<bool>> AddMemberToGrowthCenterAsync(int centerId, int memberId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var newMember = new GrowthCenterMember
                {
                    GrowthCenterId = centerId,
                    MemberId = memberId,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var result = await _repository.AddMemberToGrowthCenterAsync(newMember);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                Loggers.EventLogs($"Member ID {memberId} successfully added to Growth Center ID {centerId}.");

                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = "200";
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in AddMemberToGrowthCenterAsync service: {ex}");
            }
            return response;
        }
    }
}
