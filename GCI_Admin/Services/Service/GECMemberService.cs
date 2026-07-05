using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Utils;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class GECMemberService : IGECMemberService
    {
        private readonly GECMemberRepository _gecMemberRepository;
        private readonly AppDbContext _context;
        private readonly string _imageBasePath = "";
        private readonly SystemConfigRepository _systemConfig;
        private readonly MembersRepository _membersRepository;
        private readonly ICloudinaryService _cloudinaryService;


        public GECMemberService(GECMemberRepository gecMemberRepository, AppDbContext context, MembersRepository membersRepository, SystemConfigRepository systemConfigRepository, ICloudinaryService cloudinaryService)
        {
            _gecMemberRepository = gecMemberRepository;
            _context = context;
            _systemConfig = systemConfigRepository;
            _imageBasePath = SystemConfigHelper.GetImageBasePathAsync(_systemConfig).GetAwaiter().GetResult();
            _membersRepository = membersRepository;
            _cloudinaryService = cloudinaryService;
        }

        // ✅ CREATE
        public async Task<ApiResponse<GECMember>> CreateGECMemberAsync(GECMemberDto dto)
        {
            var response = new ApiResponse<GECMember>();

            try
            {
                var result = await _gecMemberRepository.CreateGECMemberAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to create GEC member";
                    return response;
                }
                string imageFolder = await SystemConfigHelper.GetImageBasePathAsync(_systemConfig);
                if (!string.IsNullOrEmpty(dto.ProfileImageBase64))
                {
                    string saved = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ProfileImageBase64), imageFolder, $"{result.Data.MemberId}", "jpg");
                    Loggers.EventLogs($"Saved profile image for deacon {result.Data.GECId} at {saved}");

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
                        Loggers.DoLogs($"Cloudinary upload failed for GEC member {result.Data.MemberId}: {ex}");
                    }
                }


                response.Data = result.Data;
                response.Message = "GEC member created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET ALL
        public async Task<ApiResponse<List<GECMember>>> GetGECMembersAsync()
        {
            var response = new ApiResponse<List<GECMember>>();

            try
            {
                var dbResponse = await _gecMemberRepository.GetGECMembersAsync();
                string imageFolder = await SystemConfigHelper.GetImageBasePathAsync(_systemConfig);
                foreach (var gecMember in dbResponse.Data)
                {
                    gecMember.Member.ProfileImage = ImageHelper.ReadImage(imageFolder, gecMember.MemberId.ToString());
                }

                response.IsSuccess = dbResponse.Success;
                response.Message = dbResponse.Message;
                response.Code = dbResponse.Success ? "200" : "400";
                response.Data = dbResponse.Data;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetGECMembersAsync Exception: {ex.Message}");
                response.IsSuccess = false;
                response.Message = "An error occurred while fetching GEC members.";
                response.Code = "500";
                response.Data = null;
            }

            return response;
        }


        // ✅ GET BY ID
        public async Task<ApiResponse<GECMember>> GetGECMemberByIdAsync(int gecId)
        {
            var response = new ApiResponse<GECMember>();

            try
            {
                var result = await _gecMemberRepository.GetGECMemberByIdAsync(gecId);

                if (result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "GEC member not found";
                    return response;
                }
                var memberResponse = await _membersRepository.GetMemberByIdAsync(result.Data.MemberId);
                string imageFolder = await SystemConfigHelper.GetImageBasePathAsync(_systemConfig);

                if (memberResponse != null && memberResponse.Data != null)
                {
                    memberResponse.Data.ProfileImage = ImageHelper.ReadImage(imageFolder, memberResponse.Data.Id.ToString());
                    result.Data.Member = memberResponse.Data;
                }


                response.Data = result.Data;
                response.Message = "GEC member retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ UPDATE
        public async Task<ApiResponse<GECMember>> UpdateGECMemberAsync( GECMemberDto dto)
        {
            var response = new ApiResponse<GECMember>();

            try
            {
                var result = await _gecMemberRepository.UpdateGECMemberAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "GEC member not found or update failed";
                    return response;
                }

                if (!string.IsNullOrEmpty(dto.ProfileImageBase64))
                {
                    string imageFolder = await SystemConfigHelper.GetImageBasePathAsync(_systemConfig);
                    string saved = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ProfileImageBase64), imageFolder, $"{result.Data.MemberId}", "jpg");
                    Loggers.EventLogs($"Saved profile image for GEC member {result.Data.GECId} on update at {saved}");

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
                        Loggers.DoLogs($"Cloudinary upload failed for GEC member {result.Data.MemberId}: {ex}");
                    }
                }

                response.Data = result.Data;
                response.Message = "GEC member updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ DELETE
        public async Task<ApiResponse<bool>> DeleteGECMemberAsync(int gecId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _gecMemberRepository.DeleteGECMemberAsync(gecId);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "GEC member not found or delete failed";
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

        // ✅ TOGGLE STATUS
        public async Task<ApiResponse<bool>> ToggleGECMemberStatusAsync(int id, bool isActive)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _gecMemberRepository.ToggleGECMemberStatusAsync(id, isActive);

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
