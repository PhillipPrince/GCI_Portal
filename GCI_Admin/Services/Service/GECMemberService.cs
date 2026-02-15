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


        public GECMemberService(GECMemberRepository gecMemberRepository, AppDbContext context)
        {
            _gecMemberRepository = gecMemberRepository;
            _context = context;
            _imageBasePath = SystemConfigHelper.GetImageBasePathAsync(_systemConfig).GetAwaiter().GetResult();

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
                foreach (var gecMember in dbResponse.Data)
                {
                    var member = _context.Members.FirstOrDefault(m => m.Id == gecMember.MemberId);
                    if (member != null)
                    {
                        gecMember.Photo = ImageHelper.ReadImage(_imageBasePath, gecMember.MemberId.ToString());
                        gecMember.FullName = $"{member.FirstName} {member.OtherNames}";
                        gecMember.Phone = member.Phone;
                        gecMember.Email = member.Email;
                    }
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
        public async Task<ApiResponse<GECMember>> UpdateGECMemberAsync(int gecId, GECMemberDto dto)
        {
            var response = new ApiResponse<GECMember>();

            try
            {
                var result = await _gecMemberRepository.UpdateGECMemberAsync(gecId, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "GEC member not found or update failed";
                    return response;
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
    }
}
