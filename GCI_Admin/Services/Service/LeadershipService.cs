using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Utils;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class LeadershipService : ILeadershipService
    {
        private readonly LeadershipRepository _leadershipRepository;
        private readonly SystemConfigRepository _systemConfigRepository;

        public LeadershipService(LeadershipRepository leadershipRepository, SystemConfigRepository systemConfigRepository)
        {
            _leadershipRepository = leadershipRepository;
            _systemConfigRepository = systemConfigRepository;
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

                string imageFolder = await SystemConfigHelper.GetImageBasePathAsync(_systemConfigRepository);
                if (dto.ProfileImageBase64 != null)
                {


                    string saved = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ProfileImageBase64), imageFolder, $"{result.Data.MemberId}", "jpg");
                    Loggers.EventLogs($"Saved profile image for deacon {result.Data.DeaconId} at {saved}");
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
                string imageFolder = await SystemConfigHelper.GetImageBasePathAsync(_systemConfigRepository);

                result.Data.Member.ProfileImage = ImageHelper.ReadImage(imageFolder,result.Data.MemberId.ToString());

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

        public async Task<ApiResponse<Elder>> CreateElderAsync(ElderDto dto)
        {
            var response = new ApiResponse<Elder>();

            try
            {
                var result = await _leadershipRepository.CreateElderAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Failed to create elder";
                    return response;
                }

                string imageFolder = await SystemConfigHelper.GetImageBasePathAsync(_systemConfigRepository);
                if (dto.ProfileImageBase64 != null)
                {
                    string saved = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ProfileImageBase64), imageFolder, $"{result.Data.MemberId}", "jpg");
                    Loggers.EventLogs($"Saved profile image for elder {result.Data.ElderId} at {saved}");
                }

                response.Data = result.Data;
                response.Message = "Elder created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<List<Elder>>> GetAllEldersAsync()
        {
            var response = new ApiResponse<List<Elder>>();

            try
            {
                var result = await _leadershipRepository.GetAllEldersAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Elders retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<Elder>> GetElderByIdAsync(int id)
        {
            var response = new ApiResponse<Elder>();

            try
            {
                var result = await _leadershipRepository.GetElderByIdAsync(id);

                if (!result.Success || result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Elder not found";
                    return response;
                }

                string imageFolder = await SystemConfigHelper.GetImageBasePathAsync(_systemConfigRepository);
                if (result.Data.Member != null)
                {
                    result.Data.Member.ProfileImage = ImageHelper.ReadImage(imageFolder, result.Data.MemberId.ToString());
                }

                response.Data = result.Data;
                response.Message = "Elder retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<Elder>> UpdateElderAsync(int id, ElderDto dto)
        {
            var response = new ApiResponse<Elder>();

            try
            {
                var result = await _leadershipRepository.UpdateElderAsync(id, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Update failed";
                    return response;
                }

                if (dto.ProfileImageBase64 != null)
                {
                    string imageFolder = await SystemConfigHelper.GetImageBasePathAsync(_systemConfigRepository);
                    string saved = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ProfileImageBase64), imageFolder, $"{result.Data.MemberId}", "jpg");
                    Loggers.EventLogs($"Saved profile image for elder {result.Data.ElderId} on update at {saved}");
                }

                response.Data = result.Data;
                response.Message = "Elder updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> DeleteElderAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _leadershipRepository.DeleteElderAsync(id);

                if (!result.Success || !result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Delete failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Elder deleted successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> ToggleElderStatusAsync(int id, bool isActive)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _leadershipRepository.ToggleElderStatusAsync(id, isActive);

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