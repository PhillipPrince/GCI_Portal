using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IAnnouncementsService
    {
        Task<ApiResponse<Notification>> CreateAnnouncementAsync(NotificationDto dto);
        Task<ApiResponse<List<Notification>>> GetAllAnnouncementsAsync();
        Task<ApiResponse<Notification>> GetAnnouncementByIdAsync(int id);
        Task<ApiResponse<Notification>> UpdateAnnouncementAsync(int id, NotificationDto dto);
        Task<ApiResponse<bool>> DeleteAnnouncementAsync(int id);
        Task<ApiResponse<bool>> ToggleAnnouncementStatusAsync(int id, bool isActive);
        Task<ApiResponse<List<NotificationGroup>>> GetAllNotificationGroupsAsync();
        Task<ApiResponse<NotificationGroup>> GetNotificationGroupByIdAsync(int id);
        Task<ApiResponse<NotificationGroup>> CreateOrUpdateNotificationGroupAsync(NotificationGroup model);
        Task<ApiResponse<bool>> DeleteNotificationGroupAsync(int id);
    }
    public class EventNotificationDto
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool SendSMS { get; set; }
        public bool SendPushNotification { get; set; }
        public bool SendEmail { get; set; }
        public DateTime NotificationTime { get; set; }
    }
}
