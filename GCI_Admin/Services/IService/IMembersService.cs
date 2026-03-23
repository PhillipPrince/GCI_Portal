using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IMembersService
    {
        // MEMBERS
        Task<ApiResponse<List<Member>>> GetAllMembersAsync();
        Task<ApiResponse<Member>> GetMemberByIdAsync(int id);
        Task<ApiResponse<Member>> UpdateMemberAsync(int id, MemberDto dto);
        Task<ApiResponse<bool>> DeleteMemberAsync(int id);
        Task<ApiResponse<Member>> CreateUserAsync(MemberDto dto);
        Task<ApiResponse<MemberAdditionalInformation>> CreateAdditionalInfoAsync(MemberAdditionalInformationDto dto);
        Task<ApiResponse<MemberAdditionalInformation>> GetAdditionalInfoByMemberIdAsync(int memberId);
        Task<ApiResponse<MemberAdditionalInformation>> UpdateAdditionalInfoAsync(int id, MemberAdditionalInformationDto dto);
        Task<ApiResponse<bool>> UpdateMemberRoleAsync(int memberId, int roleId);

    }
}