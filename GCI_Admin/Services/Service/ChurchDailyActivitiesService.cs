using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class ChurchDailyActivitiesService : IChurchDailyActivitiesService
    {
        private readonly ChurchDailyActivitiesRepository _repository;
        private readonly ILogger<ChurchDailyActivitiesService> _logger;

        public ChurchDailyActivitiesService(ChurchDailyActivitiesRepository repository, ILogger<ChurchDailyActivitiesService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<ChurchDailyActivity>>> GetAllAsync()
        {
            try
            {
                var activities = await _repository.GetAllAsync();
                return new ApiResponse<List<ChurchDailyActivity>>
                {
                    IsSuccess = true,
                    Code = "200",
                    Message = "Activities retrieved successfully",
                    Data = activities
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activities");
                return new ApiResponse<List<ChurchDailyActivity>>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = "Error retrieving activities: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<ChurchDailyActivity>> GetByIdAsync(int id)
        {
            try
            {
                var activity = await _repository.GetByIdAsync(id);
                if (activity == null)
                {
                    return new ApiResponse<ChurchDailyActivity>
                    {
                        IsSuccess = false,
                        Code = "404",
                        Message = "Activity not found"
                    };
                }

                return new ApiResponse<ChurchDailyActivity>
                {
                    IsSuccess = true,
                    Code = "200",
                    Message = "Activity retrieved successfully",
                    Data = activity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity with ID {Id}", id);
                return new ApiResponse<ChurchDailyActivity>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = "Error retrieving activity: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<ChurchDailyActivity>> CreateAsync(ChurchDailyActivityDto dto)
        {
            try
            {
                var activity = new ChurchDailyActivity
                {
                    DayOfWeek = dto.DayOfWeek,
                    ActivityName = dto.ActivityName,
                    Description = dto.Description,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    IsActive = dto.IsActive
                };

                var createdActivity = await _repository.CreateAsync(activity);
                
                return new ApiResponse<ChurchDailyActivity>
                {
                    IsSuccess = true,
                    Code = "201",
                    Message = "Activity created successfully",
                    Data = createdActivity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating activity");
                return new ApiResponse<ChurchDailyActivity>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = "Error creating activity: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<ChurchDailyActivity>> UpdateAsync(int id, ChurchDailyActivityDto dto)
        {
            try
            {
                var activity = new ChurchDailyActivity
                {
                    Id = id,
                    DayOfWeek = dto.DayOfWeek,
                    ActivityName = dto.ActivityName,
                    Description = dto.Description,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    IsActive = dto.IsActive
                };

                var updatedActivity = await _repository.UpdateAsync(activity);
                
                if (updatedActivity == null)
                {
                    return new ApiResponse<ChurchDailyActivity>
                    {
                        IsSuccess = false,
                        Code = "404",
                        Message = "Activity not found"
                    };
                }

                return new ApiResponse<ChurchDailyActivity>
                {
                    IsSuccess = true,
                    Code = "200",
                    Message = "Activity updated successfully",
                    Data = updatedActivity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating activity with ID {Id}", id);
                return new ApiResponse<ChurchDailyActivity>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = "Error updating activity: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var result = await _repository.DeleteAsync(id);
                if (!result)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Code = "404",
                        Message = "Activity not found"
                    };
                }

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Code = "200",
                    Message = "Activity deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting activity with ID {Id}", id);
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = "Error deleting activity: " + ex.Message
                };
            }
        }

        public async Task<ApiResponse<bool>> ToggleStatusAsync(int id, bool isActive)
        {
            try
            {
                var result = await _repository.ToggleStatusAsync(id, isActive);
                if (!result)
                {
                    return new ApiResponse<bool>
                    {
                        IsSuccess = false,
                        Code = "404",
                        Message = "Activity not found"
                    };
                }

                return new ApiResponse<bool>
                {
                    IsSuccess = true,
                    Code = "200",
                    Message = $"Activity status updated to {(isActive ? "Active" : "Inactive")}",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for activity ID {Id}", id);
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = "Error updating activity status: " + ex.Message
                };
            }
        }
    }
}
