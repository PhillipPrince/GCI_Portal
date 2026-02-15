using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IGECMemberService
    {
        Task<ApiResponse<GECMember>> CreateGECMemberAsync(GECMemberDto dto);
        Task<ApiResponse<List<GECMember>>> GetGECMembersAsync();
        Task<ApiResponse<GECMember>> GetGECMemberByIdAsync(int gecId);
        Task<ApiResponse<GECMember>> UpdateGECMemberAsync(int gecId, GECMemberDto dto);
        Task<ApiResponse<bool>> DeleteGECMemberAsync(int gecId);
    }
}
