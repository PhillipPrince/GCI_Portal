using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class AnnouncementsService : IAnnouncementsService
    {
        private readonly AnnouncementsRepository _repo;
        private readonly CommunicationService _communicationService;
        private readonly AppDbContext _context;

        public AnnouncementsService(AnnouncementsRepository repo, CommunicationService communicationService, AppDbContext context)
        {
            _repo = repo;
            _communicationService = communicationService;
            _context = context;
        }

        public async Task<ApiResponse<Notification>> CreateAnnouncementAsync(NotificationDto dto)
        {
            var response = new ApiResponse<Notification>();

            try
            {
                var result = await _repo.CreateAnnouncementAsync(dto);

                if (!result.Success)
                {
                    return new ApiResponse<Notification>
                    {
                        IsSuccess = false,
                        Code = "400",
                        Message = result.Message
                    };
                }

                // Fire-and-forget SMS (DO NOT block API)
                if (dto.SendSMS)
                {
                    _ = Task.Run(() => SendPersonalizedSmsAsync(new SendSmsDto
                    {
                        Title = dto.Title,
                        Message = dto.Message,
                        SendSMS = dto.SendSMS
                    }));
                }

                return new ApiResponse<Notification>
                {
                    IsSuccess = true,
                    Data = result.Data,
                    Message = "Announcement created successfully"
                };
            }
            catch (Exception ex)
            {
                // Replace with ILogger in real apps
                Console.WriteLine(ex);

                return new ApiResponse<Notification>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = "An error occurred while creating announcement"
                };
            }
        }
        private async Task SendPersonalizedSmsAsync(SendSmsDto dto)
        {
            Loggers.EventLogs($"Starting SMS sending process for {dto.Title}...");
            try
            {
                var members = await _context.Members
                    .AsNoTracking() // 🔥 performance boost
                    .Where(u => !string.IsNullOrEmpty(u.Phone))
                    .Select(u => new
                    {
                        Name = string.IsNullOrWhiteSpace(u.FirstName) ? "Member" : u.FirstName,
                        Phone = u.Phone.StartsWith("0")
                            ? "254" + u.Phone.Substring(1)
                            : u.Phone
                    })
                    .ToListAsync();

                var uniqueMembers = members
                    .GroupBy(m => m.Phone)
                    .Select(g => g.First())
                    .ToList();

                int batchSize = 10;

                for (int i = 0; i < uniqueMembers.Count; i += batchSize)
                {
                    var batch = uniqueMembers.Skip(i).Take(batchSize);

                    var tasks = batch.Select(async member =>
                    {
                        try
                        {
                            var message = $"Dear {member.Name}: {dto.Title}\n{dto.Message}";
                            await _communicationService.SendSmsAsync(member.Phone, message);
                        }
                        catch (Exception ex)
                        {
                            Loggers.DoLogs($"SMS failed for {member.Phone}: {ex.Message}");
                        }
                    });

                    await Task.WhenAll(tasks);

                    await Task.Delay(500);
                }
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"SMS process failed: {ex.Message}");
            }
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
