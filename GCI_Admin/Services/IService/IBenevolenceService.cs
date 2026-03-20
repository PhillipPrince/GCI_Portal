using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IBenevolenceService
    {
        Task<ApiResponse<List<BenevolenceMember>>> GetAllBenevolenceMembersAsync();
        Task<ApiResponse<BenevolenceMember>> GetBenevolenceMemberByIdAsync(int id);
        //Task<ApiResponse<BenevolenceMember>> CreateBenevolenceMemberAsync(BenevolenceMemberDto dto);
        //Task<ApiResponse<BenevolenceMember>> UpdateBenevolenceMemberAsync(int id, BenevolenceMemberDto dto);
        Task<ApiResponse<bool>> DeleteBenevolenceMemberAsync(int id);
        Task<ApiResponse<bool>> ToggleBenevolenceMemberStatusAsync(int id, bool isActive);
        Task<ApiResponse<List<BenevolenceBeneficiary>>> GetBenevolenceBeneficiariesAsync(int benId);
    }
}