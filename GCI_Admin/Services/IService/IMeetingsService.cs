
using GCI_Admin.Models;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IMeetingsService
    {
        Task<ApiResponse<List<MeetingAttendance>>> GetAllMeetingsAsync();
        Task<ApiResponse<MeetingFullDetails>> GetMeetingDetailsByIdAsync(int id);


    }
}