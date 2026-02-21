using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class GrowthCentersService : IGrowthCentersService
    {
        private readonly GrowthCentersRepository _repository;
        private readonly AppDbContext _context;

        public GrowthCentersService(GrowthCentersRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // ✅ CREATE GROWTH CENTER
        public async Task<ApiResponse<GrowthCenter>> CreateGrowthCenterAsync(GrowthCenterDto dto)
        {
            var response = new ApiResponse<GrowthCenter>();

            try
            {
                var result = await _repository.CreateGrowthCenterAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to create growth center";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Growth center created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET ALL GROWTH CENTERS
        public async Task<ApiResponse<List<GrowthCenter>>> GetAllGrowthCentersAsync()
        {
            var response = new ApiResponse<List<GrowthCenter>>();

            try
            {
                var result = await _repository.GetAllGrowthCentersAsync();

                response.Data = result.Data;
                response.Message = "Growth centers retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET GROWTH CENTER BY ID
        public async Task<ApiResponse<GrowthCenter>> GetGrowthCenterByIdAsync(int id)
        {
            var response = new ApiResponse<GrowthCenter>();

            try
            {
                var result = await _repository.GetGrowthCenterByIdAsync(id);

                if (result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Growth center not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Growth center retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ UPDATE GROWTH CENTER
        public async Task<ApiResponse<GrowthCenter>> UpdateGrowthCenterAsync(int id, GrowthCenterDto dto)
        {
            var response = new ApiResponse<GrowthCenter>();

            try
            {
                var result = await _repository.UpdateGrowthCenterAsync(id, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Growth center not found or update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Growth center updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ DELETE GROWTH CENTER
        public async Task<ApiResponse<bool>> DeleteGrowthCenterAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _repository.DeleteGrowthCenterAsync(id);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Growth center not found or delete failed";
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

        // ✅ TOGGLE ACTIVE STATUS
        public async Task<ApiResponse<bool>> ToggleGrowthCenterStatusAsync(int id, bool isActive)
        {
            var center = await _context.GrowthCenters.FindAsync(id);

            if (center == null)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "404",
                    Message = "Growth center not found"
                };
            }

            center.IsActive = isActive;
            center.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                IsSuccess = true,
                Code = "200",
                Message = isActive ? "Growth center activated successfully." : "Growth center deactivated successfully.",
                Data = true
            };
        }

        // ✅ GET ALL GROWTH CENTER LEADERS
        public async Task<ApiResponse<List<GrowthCenterLeader>>> GetAllGrowthCenterLeadersAsync()
        {
            var response = new ApiResponse<List<GrowthCenterLeader>>();

            try
            {
                var result = await _repository.GetGrowthCenterLeadersAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to retrieve growth center leaders";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Growth center leaders retrieved successfully";
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