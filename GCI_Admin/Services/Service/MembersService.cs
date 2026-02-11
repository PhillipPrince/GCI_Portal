using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class MembersService : IMembersService
    {
        private readonly MembersRepository _membersRepository;

        public MembersService(MembersRepository membersRepository)
        {
            _membersRepository = membersRepository;
        }

        // ✅ GET ALL MEMBERS
        public async Task<ApiResponse<List<Member>>> GetAllMembersAsync()
        {
            var response = new ApiResponse<List<Member>>();

            try
            {
                var result = await _membersRepository.GetAllMembersAsync();

                response.Data = result.Data;
                response.Message = "Members retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ UPDATE MEMBER
        public async Task<ApiResponse<Member>> UpdateMemberAsync(int id, MemberDto dto)
        {
            var response = new ApiResponse<Member>();

            try
            {
                var result = await _membersRepository.UpdateMemberAsync(id, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Member not found or update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Member updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ DELETE MEMBER
        public async Task<ApiResponse<bool>> DeleteMemberAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _membersRepository.DeleteMemberAsync(id);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Member not found or delete failed";
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
