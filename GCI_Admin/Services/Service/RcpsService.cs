using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using NuGet.Protocol.Core.Types;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class RcpsService : IRcpsService
    {
        private readonly RcpsRepository _rcpsRepository;

        public RcpsService(RcpsRepository rcpsRepository)
        {
            _rcpsRepository = rcpsRepository;
        }

      
        public async Task<ApiResponse<Rcps>> CreateRcpsAsync(RcpsDto dto)
        {
            var response = new ApiResponse<Rcps>();

            try
            {
                var result = await _rcpsRepository.CreateRcpsAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Failed to create Rcps";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

       
        public async Task<ApiResponse<List<Rcps>>> GetAllRcpsAsync()
        {
            var response = new ApiResponse<List<Rcps>>();

            try
            {
                var result = await _rcpsRepository.GetAllRcpsAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        
        public async Task<ApiResponse<Rcps>> GetRcpsByIdAsync(int id)
        {
            var response = new ApiResponse<Rcps>();

            try
            {
                var result = await _rcpsRepository.GetRcpsByIdAsync(id);

                if (!result.Success || result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Rcps not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

      
        public async Task<ApiResponse<Rcps>> UpdateRcpsAsync(Rcps dto)
        {
            var response = new ApiResponse<Rcps>();

            try
            {
                var result = await _rcpsRepository.UpdateRcpsAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        
        public async Task<ApiResponse<bool>> DeleteRcpsAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _rcpsRepository.DeleteRcpsAsync(id);

                if (!result.Success || !result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Delete failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Rcps deleted successfully";
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
        // ✅ CREATE
        // =========================================================
        public async Task<ApiResponse<RcpsPledges>> CreateRcpsPledgeAsync(RcpsPledgesDto dto)
        {
            var response = new ApiResponse<RcpsPledges>();

            try
            {
                var result = await _rcpsRepository.CreateRcpsPledgeAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledge created successfully";
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
        // ✅ GET ALL
        // =========================================================
        public async Task<ApiResponse<List<RcpsPledges>>> GetAllRcpsPledgesAsync()
        {
            var response = new ApiResponse<List<RcpsPledges>>();

            try
            {
                var result = await _rcpsRepository.GetAllRcpsPledgesAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledges retrieved successfully";
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
        public async Task<ApiResponse<RcpsPledges>> GetRcpsPledgeByIdAsync(int id)
        {
            var response = new ApiResponse<RcpsPledges>();

            try
            {
                var result = await _rcpsRepository.GetRcpsPledgeByIdAsync(id);

                if (!result.Success || result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledge retrieved successfully";
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
        public async Task<ApiResponse<RcpsPledges>> UpdateRcpsPledgeAsync(int id, RcpsPledgesDto dto)
        {
            var response = new ApiResponse<RcpsPledges>();

            try
            {
                var result = await _rcpsRepository.UpdateRcpsPledgeAsync(id, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledge updated successfully";
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
        // ✅ DELETE
        // =========================================================
        public async Task<ApiResponse<bool>> DeleteRcpsPledgeAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _rcpsRepository.DeleteRcpsPledgeAsync(id);

                if (!result.Success || !result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledge deleted successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<ApiResponse<List<RcpsPledges>>> GetPledgesByRcpsIdAsync(int id)
        {
            var response = new ApiResponse<List<RcpsPledges>>();

            try
            {
                var result = await _rcpsRepository.GetPledgesByRcpsIdAsync(id);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Pledges retrieved successfully";
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