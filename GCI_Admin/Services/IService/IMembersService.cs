using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IMembersService
    {
        Task<ApiResponse<List<Member>>> GetAllMembersAsync();
        Task<ApiResponse<Member>> UpdateMemberAsync(int id, MemberDto dto);
        Task<ApiResponse<bool>> DeleteMemberAsync(int id);
        Task<ApiResponse<Member>> CreateUserAsync(MemberDto dto);
        Task<ApiResponse<MembershipClass>> CreateMembershipClassAsync(MembershipClassDto dto);
    }
}
