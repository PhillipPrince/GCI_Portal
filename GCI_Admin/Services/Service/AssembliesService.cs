using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.EntityFrameworkCore;
using Utils;
using GCI_Admin.Utils;

namespace GCI_Admin.Services.Service
{
    public class AssembliesService : IAssembliesService
    {
        private readonly AssembliesRepository _assembliesRepository;
        private readonly AppDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;

        public AssembliesService(AssembliesRepository assembliesRepository, AppDbContext context, ICloudinaryService cloudinaryService)
        {
            _assembliesRepository = assembliesRepository;
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

        // ✅ CREATE ASSEMBLY
        public async Task<ApiResponse<Assembly>> CreateAssemblyAsync(AssemblyDto dto)
        {
            var response = new ApiResponse<Assembly>();

            try
            {
                var result = await _assembliesRepository.CreateAssemblyAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to create assembly";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Assembly created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET ALL ASSEMBLIES
        public async Task<ApiResponse<List<Assembly>>> GetAllAssembliesAsync()
        {
            var response = new ApiResponse<List<Assembly>>();

            try
            {
                var result = await _assembliesRepository.GetAllAssembliesAsync();

                response.Data = result.Data;
                response.Message = "Assemblies retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET ASSEMBLY BY ID
        public async Task<ApiResponse<Assembly>> GetAssemblyByIdAsync(int assemblyId)
        {
            var response = new ApiResponse<Assembly>();

            try
            {
                var result = await _assembliesRepository.GetAssemblyByIdAsync(assemblyId);

                if (result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Assembly not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Assembly retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ UPDATE ASSEMBLY
        public async Task<ApiResponse<Assembly>> UpdateAssemblyAsync(int assemblyId, AssemblyDto dto)
        {
            var response = new ApiResponse<Assembly>();

            try
            {
                var result = await _assembliesRepository.UpdateAssemblyAsync(assemblyId, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Assembly not found or update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Assembly updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ DELETE ASSEMBLY (soft-delete)
        public async Task<ApiResponse<bool>> DeleteAssemblyAsync(int assemblyId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _assembliesRepository.DeleteAssemblyAsync(assemblyId);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Assembly not found or delete failed";
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
        public async Task<ApiResponse<bool>> ToggleAssemblyStatusAsync(int assemblyId, bool isActive)
        {
            var assembly = await _context.Assemblies.FindAsync(assemblyId);

            if (assembly == null)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "404",
                    Message = "Assembly not found"
                };
            }

            //assembly.IsActive = isActive;
            //assembly.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                IsSuccess = true,
                Code = "200",
                Message = isActive ? "Assembly activated successfully." : "Assembly deactivated successfully.",
                Data = true
            };
        }

        // ✅ GET ALL ACTIVE ASSEMBLY LEADERS
        public async Task<ApiResponse<List<AssemblyLeader>>> GetAllAssemblyLeadersAsync()
        {
            var response = new ApiResponse<List<AssemblyLeader>>();
            try
            {
                var result = await _assembliesRepository.GetAssemblyLeadersAsync();
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to retrieve assembly leaders";
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
                response.Message = "Assembly leaders retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        // ✅ CREATE ASSEMBLY LEADER
        public async Task<ApiResponse<AssemblyLeader>> CreateAssemblyLeaderAsync(AssemblyLeaderDto dto)
        {
            var response = new ApiResponse<AssemblyLeader>();
            try
            {
                var result = await _assembliesRepository.CreateAssemblyLeaderAsync(dto);
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
                    Loggers.EventLogs($"Saved profile image for assembly leader {result.Data.AssemblyLeaderId} at {saved}");

                    try
                    {
                        var cloudinaryUrl = await _cloudinaryService.UploadBase64ImageAsync(dto.ProfileImageBase64);
                        if (!string.IsNullOrEmpty(cloudinaryUrl))
                        {
                            var memberToUpdate = await _context.Members.FindAsync(result.Data.MemberId);
                            if (memberToUpdate != null)
                            {
                                memberToUpdate.ProfilePictureUrl = cloudinaryUrl;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Loggers.DoLogs($"Cloudinary upload failed for assembly leader member {result.Data.MemberId}: {ex}");
                    }
                }

                response.Data = result.Data;
                response.Message = "Assembly leader assigned successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        // ✅ GET ASSEMBLY LEADER BY ID
        public async Task<ApiResponse<AssemblyLeader>> GetAssemblyLeaderByIdAsync(int id)
        {
            var response = new ApiResponse<AssemblyLeader>();
            try
            {
                var result = await _assembliesRepository.GetAssemblyLeaderByIdAsync(id);
                if (!result.Success || result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Assembly leader not found";
                    return response;
                }

                var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                if (result.Data.Member != null)
                {
                    result.Data.Member.ProfileImage = ImageHelper.ReadImage(imageFolder, result.Data.MemberId.ToString());
                }

                response.Data = result.Data;
                response.Message = "Assembly leader retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        // ✅ UPDATE ASSEMBLY LEADER
        public async Task<ApiResponse<AssemblyLeader>> UpdateAssemblyLeaderAsync(int id, AssemblyLeaderDto dto)
        {
            var response = new ApiResponse<AssemblyLeader>();
            try
            {
                var result = await _assembliesRepository.UpdateAssemblyLeaderAsync(id, dto);
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
                    Loggers.EventLogs($"Saved profile image for assembly leader {result.Data.AssemblyLeaderId} on update at {saved}");

                    try
                    {
                        var cloudinaryUrl = await _cloudinaryService.UploadBase64ImageAsync(dto.ProfileImageBase64);
                        if (!string.IsNullOrEmpty(cloudinaryUrl))
                        {
                            var memberToUpdate = await _context.Members.FindAsync(result.Data.MemberId);
                            if (memberToUpdate != null)
                            {
                                memberToUpdate.ProfilePictureUrl = cloudinaryUrl;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Loggers.DoLogs($"Cloudinary upload failed for assembly leader member {result.Data.MemberId}: {ex}");
                    }
                }

                response.Data = result.Data;
                response.Message = "Assembly leader updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        // ✅ DELETE ASSEMBLY LEADER
        public async Task<ApiResponse<bool>> DeleteAssemblyLeaderAsync(int id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var result = await _assembliesRepository.DeleteAssemblyLeaderAsync(id);
                if (!result.Success || !result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Delete failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Assembly leader deleted successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        // ✅ TOGGLE STATUS
        public async Task<ApiResponse<bool>> ToggleAssemblyLeaderStatusAsync(int id, bool isActive)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var result = await _assembliesRepository.ToggleAssemblyLeaderStatusAsync(id, isActive);
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
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
    }
}


