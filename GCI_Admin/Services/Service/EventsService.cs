using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class EventsService : IEventsService
    {
        private readonly EventsRepository _eventsRepository;

        public EventsService(EventsRepository eventsRepository)
        {
            _eventsRepository = eventsRepository;
        }

        public async Task<ApiResponse<Event>> CreateEventAsync(EventDto dto)
        {
            var response = new ApiResponse<Event>();

            try
            {
                var result = await _eventsRepository.CreateEventAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to create event";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Event created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<List<Event>>> GetAllEventsAsync()
        {
            var response = new ApiResponse<List<Event>>();

            try
            {
                var result = await _eventsRepository.GetAllEventsAsync();

                response.Data = result.Data;
                response.Message = "Events retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<Event>> GetEventByIdAsync(int eventId)
        {
            var response = new ApiResponse<Event>();

            try
            {
                var result = await _eventsRepository.GetEventByIdAsync(eventId);

                if (result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Event not found";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Event retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<Event>> UpdateEventAsync(int eventId, EventDto dto)
        {
            var response = new ApiResponse<Event>();

            try
            {
                var result = await _eventsRepository.UpdateEventAsync(eventId, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Event not found or update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Event updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> DeleteEventAsync(int eventId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _eventsRepository.DeleteEventAsync(eventId);

                if (!result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Event not found or delete failed";
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
