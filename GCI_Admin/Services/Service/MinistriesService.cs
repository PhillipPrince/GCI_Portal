using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.EntityFrameworkCore;
using Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GCI_Admin.Services.Service
{
    public class MinistriesService : IMinistriesService
    {
        private readonly MinistriesRepository _ministriesRepository;
        private readonly AppDbContext _context;

        public MinistriesService(MinistriesRepository ministriesRepository, AppDbContext context)
        {
            _ministriesRepository = ministriesRepository;
            _context = context;
        }

        // ✅ CREATE MINISTRY
        public async Task<ApiResponse<Ministry>> CreateMinistryAsync(MinistryDto dto)
        {
            var response = new ApiResponse<Ministry>();

            try
            {
                var result = await _ministriesRepository.CreateMinistryAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to create ministry";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Ministry created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET ALL MINISTRIES
        public async Task<ApiResponse<List<Ministry>>> GetAllMinistriesAsync()
        {
            var response = new ApiResponse<List<Ministry>>();

            try
            {
                var result = await _ministriesRepository.GetAllMinistriesAsync();

                response.Data = result.Data;
                response.Message = "Ministries retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ GET MINISTRY BY ID
        public async Task<ApiResponse<Ministry>> GetMinistryByIdAsync(int ministryId)
        {
            var response = new ApiResponse<Ministry>();

            try
            {
                var result = await _ministriesRepository.GetMinistryByIdAsync(ministryId);

                if (result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Ministry not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Ministry retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ UPDATE MINISTRY
        public async Task<ApiResponse<Ministry>> UpdateMinistryAsync(int ministryId, MinistryDto dto)
        {
            var response = new ApiResponse<Ministry>();

            try
            {
                var result = await _ministriesRepository.UpdateMinistryAsync(ministryId, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Ministry not found or update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Ministry updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ DELETE MINISTRY (soft-delete)
        public async Task<ApiResponse<bool>> DeleteMinistryAsync(int ministryId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _ministriesRepository.DeleteMinistryAsync(ministryId);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Ministry not found or delete failed";
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
        public async Task<ApiResponse<bool>> ToggleMinistryStatusAsync(int ministryId, bool isActive)
        {
            var ministry = await _context.Ministries.FindAsync(ministryId);

            if (ministry == null)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "404",
                    Message = "Ministry not found"
                };
            }

            ministry.IsActive = isActive;
            ministry.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                IsSuccess = true,
                Code = "200",
                Message = isActive ? "Ministry activated successfully." : "Ministry deactivated successfully.",
                Data = true
            };
        }

        // ✅ GET ALL MINISTRY LEADERS
        public async Task<ApiResponse<List<MinistryLeader>>> GetAllMinistryLeadersAsync()
        {
            var response = new ApiResponse<List<MinistryLeader>>();

            try
            {
                var result = await _ministriesRepository.GetMinistryLeadersAsync();

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to retrieve ministry leaders";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Ministry leaders retrieved successfully";
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