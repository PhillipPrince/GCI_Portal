using Azure;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IEventsService
    {
        Task<ApiResponse<Event>> CreateEventAsync(EventDto dto);

        Task<ApiResponse<List<Event>>> GetAllEventsAsync();

        Task<ApiResponse<Event>> GetEventByIdAsync(int eventId);

        Task<ApiResponse<Event>> UpdateEventAsync(int eventId, EventDto dto);
        Task<ApiResponse<Event>> UpdateEventAgeGroupsAsync(int eventId, string ageGroups);

        Task<ApiResponse<bool>> DeleteEventAsync(int eventId);
        Task<ApiResponse<List<EventRegistration>>> GetEventRegistrationsAsync();
        Task<ApiResponse<EventUploadResponse>> ProcessEventExcelUploadAsync(IFormFile file, string createdBy, string uploadOption);
        Task<ApiResponse<bool>> ToggleEventStatusAsync(int eventId, bool isActive);
                Task<ApiResponse<List<AnnualTheme>>> GetAllAnnualThemesAsync(string? assemblyName = null);
        Task<ApiResponse<bool>> DeleteAnnualThemeAsync(int id);
        Task<ApiResponse<List<MonthlyTheme>>> GetAllMonthlyThemesAsync(string? assemblyName = null);
        Task<ApiResponse<bool>> DeleteMonthlyThemeAsync(int id);
        Task<ApiResponse<MonthlyTheme>> GetCurrentMonthlyThemeAsync(string? assemblyName = null);
        Task<ApiResponse<AnnualTheme>> GetCurrentYearThemeAsync(string? assemblyName = null);
        Task<ApiResponse<MonthlyTheme>> UpdateMonthlyThemeAsync(int id, MonthlyThemeDto dto, string? assemblyName = null);
        Task<ApiResponse<AnnualTheme>> UpdateAnnualThemeAsync(int id, AnnualThemeDto dto, string? assemblyName = null);
        Task<ApiResponse<List<Event>>> GetUpcomingEventsAsync();
        Task<ApiResponse<List<Event>>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<ApiResponse<List<EventRegistration>>> GetEventRegistrationsByEventIdAsync(int eventId);

        Task<ApiResponse<string>> SendPaymentReminderAsync(int id);
        Task<ApiResponse<string>> SendBulkPaymentRemindersAsync(int eventId);
        Task<ApiResponse<string>> SendAttendanceReminderAsync(int id);
        Task<ApiResponse<string>> SendBulkAttendanceRemindersAsync(int eventId);

        Task<ApiResponse<object>> CheckEventRegistrationAsync(string phone, int eventId);
        Task<ApiResponse<int>> CheckPaymentStatusAsync(int registrationId);
        Task<ApiResponse<bool>> UsherSubmitRegistrationAsync(GCI_Admin.Controllers.UsherRegistrationDto dto);
    }
}
