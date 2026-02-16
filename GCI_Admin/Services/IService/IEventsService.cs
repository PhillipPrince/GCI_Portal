using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IEventsService
    {
        Task<ApiResponse<Event>> CreateEventAsync(EventDto dto);

        Task<ApiResponse<List<Event>>> GetAllEventsAsync();

        Task<ApiResponse<Event>> GetEventByIdAsync(int eventId);

        Task<ApiResponse<Event>> UpdateEventAsync(int eventId, EventDto dto);

        Task<ApiResponse<bool>> DeleteEventAsync(int eventId);
        Task<ApiResponse<List<EventRegistration>>> GetEventRegistrationsAsync();
    }
}
