using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class BenevolenceService : IBenevolenceService
    {
        private readonly BenevolenceRepository _benevolenceRepository;
        private readonly AppDbContext _context;

        public BenevolenceService(BenevolenceRepository benevolenceRepository, AppDbContext context)
        {
            _benevolenceRepository = benevolenceRepository;
            _context = context;
        }

        public async Task<ApiResponse<List<BenevolenceMember>>> GetAllBenevolenceMembersAsync()
        {
            var response = new ApiResponse<List<BenevolenceMember>>();

            try
            {
                var result = await _benevolenceRepository.GetAllBenevolenceMembersAsync();

                response.Data = result.Data;
                response.Message = "Benevolence members retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<ApiResponse<List<BenevolenceBeneficiary>>> GetBenevolenceBeneficiariesAsync(int benId)
        {
            var response = new ApiResponse<List<BenevolenceBeneficiary>>();

            try
            {
                if (benId <= 0)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Invalid benevolence member ID";
                    return response;
                }
                var result = await _benevolenceRepository.GetBenevolenceBeneficiariesAsync(benId);

                response.Data = result.Data;
                response.Message = "Benevolence beneficiaries retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<ApiResponse<BenevolenceMember>> GetBenevolenceMemberByIdAsync(int id)
        {
            var response = new ApiResponse<BenevolenceMember>();

            try
            {
                var result = await _benevolenceRepository.GetBenevolenceMemberByIdAsync(id);

                if (result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Benevolence member not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Benevolence member retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        //public async Task<ApiResponse<BenevolenceMember>> CreateBenevolenceMemberAsync(BenevolenceMemberDto dto)
        //{
        //    var response = new ApiResponse<BenevolenceMember>();

        //    try
        //    {
        //        var result = await _benevolenceRepository.CreateBenevolenceMemberAsync(dto);

        //        if (!result.Success)
        //        {
        //            response.IsSuccess = false;
        //            response.Code = "400";
        //            response.Message = "Failed to create benevolence member";
        //            return response;
        //        }

        //        response.Data = result.Data;
        //        response.Message = "Benevolence member created successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Code = "500";
        //        response.Message = ex.Message;
        //    }

        //    return response;
        //}

        //public async Task<ApiResponse<BenevolenceMember>> UpdateBenevolenceMemberAsync(int id, BenevolenceMemberDto dto)
        //{
        //    var response = new ApiResponse<BenevolenceMember>();

        //    try
        //    {
        //        var result = await _benevolenceRepository.UpdateBenevolenceMemberAsync(id, dto);

        //        if (!result.Success)
        //        {
        //            response.IsSuccess = false;
        //            response.Code = "404";
        //            response.Message = "Benevolence member not found or update failed";
        //            return response;
        //        }

        //        response.Data = result.Data;
        //        response.Message = "Benevolence member updated successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Code = "500";
        //        response.Message = ex.Message;
        //    }

        //    return response;
        //}

        public async Task<ApiResponse<bool>> DeleteBenevolenceMemberAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _benevolenceRepository.DeleteBenevolenceMemberAsync(id);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Benevolence member not found or delete failed";
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

        public async Task<ApiResponse<bool>> ToggleBenevolenceMemberStatusAsync(int id, bool isActive)
        {
            var member = await _context.BenevolenceMembers.FindAsync(id);

            if (member == null)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "404",
                    Message = "Benevolence member not found"
                };
            }

            member.IsActive = isActive;
            member.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                IsSuccess = true,
                Code = "200",
                Message = isActive ? "Member activated successfully." : "Member deactivated successfully.",
                Data = true
            };
        }
    }
}