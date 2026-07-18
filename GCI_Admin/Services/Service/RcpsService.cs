using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using NuGet.Protocol.Core.Types;
using Utils;
using GCI_Admin.Utils;
using GCI_Admin.DBOperations;
using Microsoft.EntityFrameworkCore;

namespace GCI_Admin.Services.Service
{
    public class RcpsService : IRcpsService
    {
        private readonly RcpsRepository _rcpsRepository;
        private readonly AppDbContext _context;
        private readonly ICloudinaryService _cloudinaryService;

        public RcpsService(RcpsRepository rcpsRepository, AppDbContext context, ICloudinaryService cloudinaryService)
        {
            _rcpsRepository = rcpsRepository;
            _context = context;
            _cloudinaryService = cloudinaryService;
        }

      
        public async Task<ApiResponse<Rcps>> CreateRcpsAsync(RcpsDto dto)
        {
            var response = new ApiResponse<Rcps>();

            try
            {
                var result = await _rcpsRepository.CreateRcpsAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Failed to create Rcps";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

       
        public async Task<ApiResponse<List<Rcps>>> GetAllRcpsAsync()
        {
            var response = new ApiResponse<List<Rcps>>();

            try
            {
                var result = await _rcpsRepository.GetAllRcpsAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        
        public async Task<ApiResponse<Rcps>> GetRcpsByIdAsync(int id)
        {
            var response = new ApiResponse<Rcps>();

            try
            {
                var result = await _rcpsRepository.GetRcpsByIdAsync(id);

                if (!result.Success || result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Rcps not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

      
        public async Task<ApiResponse<Rcps>> UpdateRcpsAsync(Rcps dto)
        {
            var response = new ApiResponse<Rcps>();

            try
            {
                var result = await _rcpsRepository.UpdateRcpsAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        
        public async Task<ApiResponse<bool>> DeleteRcpsAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _rcpsRepository.DeleteRcpsAsync(id);

                if (!result.Success || !result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Delete failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps deleted successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }
        // =========================================================
        // ? CREATE
        // =========================================================
        public async Task<ApiResponse<RcpsPledges>> CreateRcpsPledgeAsync(RcpsPledgesDto dto)
        {
            var response = new ApiResponse<RcpsPledges>();

            try
            {
                var result = await _rcpsRepository.CreateRcpsPledgeAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledge created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // =========================================================
        // ? GET ALL
        // =========================================================
        public async Task<ApiResponse<List<RcpsPledges>>> GetAllRcpsPledgesAsync()
        {
            var response = new ApiResponse<List<RcpsPledges>>();

            try
            {
                var result = await _rcpsRepository.GetAllRcpsPledgesAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledges retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // =========================================================
        // ? GET BY ID
        // =========================================================
        public async Task<ApiResponse<RcpsPledges>> GetRcpsPledgeByIdAsync(int id)
        {
            var response = new ApiResponse<RcpsPledges>();

            try
            {
                var result = await _rcpsRepository.GetRcpsPledgeByIdAsync(id);

                if (!result.Success || result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledge retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // =========================================================
        // ? UPDATE
        // =========================================================
        public async Task<ApiResponse<RcpsPledges>> UpdateRcpsPledgeAsync(int id, RcpsPledgesDto dto)
        {
            var response = new ApiResponse<RcpsPledges>();

            try
            {
                var result = await _rcpsRepository.UpdateRcpsPledgeAsync(id, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledge updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // =========================================================
        // ? DELETE
        // =========================================================
        public async Task<ApiResponse<bool>> DeleteRcpsPledgeAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _rcpsRepository.DeleteRcpsPledgeAsync(id);

                if (!result.Success || !result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledge deleted successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<ApiResponse<List<RcpsPledges>>> GetPledgesByRcpsIdAsync(int id)
        {
            var response = new ApiResponse<List<RcpsPledges>>();

            try
            {
                var result = await _rcpsRepository.GetPledgesByRcpsIdAsync(id);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledges retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // =========================================================
        // COUNTY COORDINATORS CRUD
        // =========================================================

        public async Task<ApiResponse<RcpsCountyCoordinator>> CreateRcpsCountyCoordinatorAsync(RcpsCountyCoordinatorDto dto)
        {
            var response = new ApiResponse<RcpsCountyCoordinator>();

            try
            {
                var result = await _rcpsRepository.CreateRcpsCountyCoordinatorAsync(dto);

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
                    Loggers.EventLogs($"Saved profile image for county coordinator {result.Data.RcpsCountyCoordinatorId} at {saved}");

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
                        Loggers.DoLogs($"Cloudinary upload failed for county coordinator {result.Data.RcpsCountyCoordinatorId}: {ex}");
                    }
                }

                response.Data = result.Data;
                response.Message = "County Coordinator assigned successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = "An error occurred while creating county coordinator";
                Loggers.DoLogs($"Error in CreateRcpsCountyCoordinatorAsync service: {ex}");
            }

            return response;
        }

        public async Task<ApiResponse<List<RcpsCountyCoordinator>>> GetAllRcpsCountyCoordinatorsAsync()
        {
            var response = new ApiResponse<List<RcpsCountyCoordinator>>();
            try
            {
                var result = await _rcpsRepository.GetAllRcpsCountyCoordinatorsAsync();
                if (result.Success)
                {
                    response.Data = result.Data;
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = result.Message;
                    response.Code = "400";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Code = "500";
            }
            return response;
        }

        public async Task<ApiResponse<RcpsCountyCoordinator>> GetRcpsCountyCoordinatorByIdAsync(int id)
        {
            var response = new ApiResponse<RcpsCountyCoordinator>();
            try
            {
                var result = await _rcpsRepository.GetRcpsCountyCoordinatorByIdAsync(id);
                if (result.Success)
                {
                    response.Data = result.Data;
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = result.Message;
                    response.Code = "404";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Code = "500";
            }
            return response;
        }

        public async Task<ApiResponse<List<RcpsCountyCoordinator>>> GetRcpsCountyCoordinatorsByRcpsAsync(int rcpsId)
        {
            var response = new ApiResponse<List<RcpsCountyCoordinator>>();
            try
            {
                var result = await _rcpsRepository.GetRcpsCountyCoordinatorsByRcpsAsync(rcpsId);
                if (result.Success)
                {
                    response.Data = result.Data;
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = result.Message;
                    response.Code = "400";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Code = "500";
            }
            return response;
        }

        public async Task<ApiResponse<RcpsCountyCoordinator>> UpdateRcpsCountyCoordinatorAsync(RcpsCountyCoordinatorDto dto)
        {
            var response = new ApiResponse<RcpsCountyCoordinator>();
            try
            {
                var result = await _rcpsRepository.UpdateRcpsCountyCoordinatorAsync(dto);
                if (result.Success)
                {
                    response.Data = result.Data;
                    response.IsSuccess = true;
                    response.Message = result.Message;
                    
                    var imageFolderSetting = await _context.SystemConfig.FirstOrDefaultAsync(x => x.ConfigKey == "ImageBasePath");
                    string imageFolder = imageFolderSetting?.ConfigValue ?? "wwwroot/uploads";
                    if (!string.IsNullOrEmpty(dto.ProfileImageBase64))
                    {
                        string saved = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ProfileImageBase64), imageFolder, $"{result.Data.MemberId}", "jpg");
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
                            Loggers.DoLogs($"Cloudinary upload failed for county coordinator {result.Data.RcpsCountyCoordinatorId} on update: {ex}");
                        }
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = result.Message;
                    response.Code = "400";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Code = "500";
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteRcpsCountyCoordinatorAsync(int id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var result = await _rcpsRepository.DeleteRcpsCountyCoordinatorAsync(id);
                if (result.Success)
                {
                    response.Data = true;
                    response.IsSuccess = true;
                    response.Message = result.Message;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = result.Message;
                    response.Code = "400";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Code = "500";
            }
            return response;
        }
        
        public async Task<ApiResponse<bool>> ToggleCountyCoordinatorStatusAsync(int id, bool isActive)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var result = await _rcpsRepository.ToggleCountyCoordinatorStatusAsync(id, isActive);
                if (result.Success)
                {
                    response.Data = true;
                    response.IsSuccess = true;
                    response.Message = result.Message;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = result.Message;
                    response.Code = "400";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Code = "500";
            }
            return response;
        }

        public async Task<ApiResponse<List<County>>> GetAllCountiesAsync()
        {
            var response = new ApiResponse<List<County>>();
            try
            {
                var counties = await _context.Counties.OrderBy(c => c.CountyName).ToListAsync();
                response.Data = counties;
                response.IsSuccess = true;
                response.Message = "Counties retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<List<RcpCountyMember>>> GetRcpCountyMembersByRcpsAsync(int rcpsId)
        {
            var response = new ApiResponse<List<RcpCountyMember>>();
            try
            {
                var result = await _rcpsRepository.GetRcpCountyMembersByRcpsAsync(rcpsId);
                
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "RCP county members retrieved successfully";
                response.Code = "200";
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in GetRcpCountyMembersByCountyAsync service: {ex}");
            }
            return response;
        }

        public async Task<ApiResponse<bool>> AddMemberToRcpCountyAsync(int rcpsId, int memberId)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var newMember = new RcpCountyMember
                {
                    RcpsId = rcpsId,
                    MemberId = memberId,
                    IsLeader = false,
                    Status = "Active",
                    CreatedAt = DateTime.Now
                };

                var result = await _rcpsRepository.AddMemberToRcpCountyAsync(newMember);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                Loggers.EventLogs($"Member ID {memberId} successfully added to RCP ID {rcpsId}.");

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
                Loggers.DoLogs($"Error in AddMemberToRcpCountyAsync service: {ex}");
            }
            return response;
        }

        public async Task<ApiResponse<RcpsInvite>> GetRcpsInviteByCodeAsync(string code)
        {
            var response = new ApiResponse<RcpsInvite>();
            try
            {
                var result = await _rcpsRepository.GetRcpsInviteByCodeAsync(code);
                
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps Invite retrieved successfully";
                response.Code = "200";
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
                Loggers.DoLogs($"Error in GetRcpsInviteByCodeAsync service: {ex}");
            }
            return response;
        }
    }
}
