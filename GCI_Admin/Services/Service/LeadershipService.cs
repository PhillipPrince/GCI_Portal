using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class LeadershipService : ILeadershipService
    {
        private readonly LeadershipRepository _leadershipRepository;

        public LeadershipService(LeadershipRepository leadershipRepository)
        {
            _leadershipRepository = leadershipRepository;
        }

        // =========================================================
        // ✅ CREATE DEACON
        // =========================================================
        public async Task<ApiResponse<Deacon>> CreateDeaconAsync(DeaconDto dto)
        {
            var response = new ApiResponse<Deacon>();

            try
            {
                var result = await _leadershipRepository.CreateDeaconAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Failed to create deacon";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Deacon created successfully";
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
        // ✅ GET ALL DEACONS
        // =========================================================
        public async Task<ApiResponse<List<Deacon>>> GetAllDeaconsAsync()
        {
            var response = new ApiResponse<List<Deacon>>();

            try
            {
                var result = await _leadershipRepository.GetAllDeaconsAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Deacons retrieved successfully";
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
        // ✅ GET BY ID
        // =========================================================
        public async Task<ApiResponse<Deacon>> GetDeaconByIdAsync(int id)
        {
            var response = new ApiResponse<Deacon>();

            try
            {
                var result = await _leadershipRepository.GetDeaconByIdAsync(id);

                if (!result.Success || result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Deacon not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Deacon retrieved successfully";
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
        // ✅ UPDATE
        // =========================================================
        public async Task<ApiResponse<Deacon>> UpdateDeaconAsync(int id, DeaconDto dto)
        {
            var response = new ApiResponse<Deacon>();

            try
            {
                var result = await _leadershipRepository.UpdateDeaconAsync(id, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Deacon updated successfully";
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
        // ✅ DELETE (SOFT)
        // =========================================================
        public async Task<ApiResponse<bool>> DeleteDeaconAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _leadershipRepository.DeleteDeaconAsync(id);

                if (!result.Success || !result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Delete failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Deacon deleted successfully";
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
        // ✅ TOGGLE DUTY STATUS
        // =========================================================
        public async Task<ApiResponse<bool>> ToggleDutyStatusAsync(int id, bool onDuty)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _leadershipRepository.ToggleDutyStatusAsync(id, onDuty);

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