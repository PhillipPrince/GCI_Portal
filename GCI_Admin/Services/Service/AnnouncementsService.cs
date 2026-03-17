using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class AnnouncementsService : IAnnouncementsService
    {
        private readonly AnnouncementsRepository _repo;

        public AnnouncementsService(AnnouncementsRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<Notification>> CreateAnnouncementAsync(NotificationDto dto)
        {
            var response = new ApiResponse<Notification>();

            try
            {
                var result = await _repo.CreateAnnouncementAsync(dto);
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.IsSuccess = true;
                response.Data = result.Data;
                response.Message = "Announcement created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<List<Notification>>> GetAllAnnouncementsAsync()
        {
            var response = new ApiResponse<List<Notification>>();

            try
            {
                var result = await _repo.GetAllAnnouncementsAsync();
                response.IsSuccess = result.Success;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<Notification>> GetAnnouncementByIdAsync(int id)
        {
            var response = new ApiResponse<Notification>();

            try
            {
                var result = await _repo.GetAnnouncementByIdAsync(id);
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message;
                    return response;
                }

                response.IsSuccess = true;
                response.Data = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<Notification>> UpdateAnnouncementAsync(int id, NotificationDto dto)
        {
            var response = new ApiResponse<Notification>();

            try
            {
                var result = await _repo.UpdateAnnouncementAsync(id, dto);
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.IsSuccess = true;
                response.Data = result.Data;
                response.Message = "Announcement updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> DeleteAnnouncementAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _repo.DeleteAnnouncementAsync(id);
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.IsSuccess = true;
                response.Data = result.Data;
                response.Message = "Announcement deleted successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> ToggleAnnouncementStatusAsync(int id, bool isActive)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _repo.ToggleAnnouncementStatusAsync(id, isActive);
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }

                response.IsSuccess = true;
                response.Data = result.Data;
                response.Message = "Status toggled";
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
