using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class AnnouncementsRepository
    {
        private readonly AppDbContext _context;

        public AnnouncementsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse<Notification>> CreateAnnouncementAsync(NotificationDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = new Notification
                {
                    Title = dto.Title,
                    Message = dto.Message,
                    IsChurchWide = dto.IsChurchWide,
                    MinistryId = dto.MinistryId,
                    NotificationTime = dto.NotificationTime,
                    ExpiryTime = dto.ExpiryTime,
                    RequiresReminder = dto.RequiresReminder,
                    SendSMS = dto.SendSMS,
                    SendEmail = dto.SendEmail,
                    CreatedAt = DateTime.Now,
                    IsActive = dto.IsActive,
                    CreatedById = dto.CreatedById.Value,
                    NotificationGroupId = dto.NotificationGroupId,
                    IsSent = false,
                    SendPushNotification = dto.SendPushNotification,
                    GrowthCenterId = dto.GrowthCenterId ?? 0,
                    RcpsId = dto.RcpsId ?? 0,
                    PushNotificationType = dto.PushNotificationType ?? "general",
                    DeepLinkScreen = dto.DeepLinkScreen ?? "notifications",
                    DeepLinkId = dto.DeepLinkId
                };

                _context.Notifications.Add(entity);
                await _context.SaveChangesAsync();

                var notificationId = entity.NotificationId;

                if (dto.NotificationGroupId == 3)
                {


                    if (dto.SelectedMembers != null && dto.SelectedMembers.Any())
                    {
                        var specialMembers = dto.SelectedMembers.Select(memberId => new SpecialNotificationMember
                        {
                            NotificationId = notificationId,
                            MemberId = memberId,
                            IsNotified = false,
                            CreatedAt = DateTime.Now
                        }).ToList();

                        await _context.SpecialNotificationMembers.AddRangeAsync(specialMembers);
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();

                return new DbResponse<Notification>
                {
                    Success = true,
                    Data = entity,
                    Message = "Announcement created successfully"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Loggers.DoLogs($"Error in CreateAnnouncementAsync: {ex}");

                return new DbResponse<Notification>
                {
                    Success = false,
                    Message = $"Error creating announcement: {ex.Message}"
                };
            }
        }
        public async Task<DbResponse<List<Notification>>> GetAllAnnouncementsAsync()
        {
            try
            {
                var list = await _context.Notifications
                    .OrderByDescending(n => n.NotificationTime)
                    .ToListAsync();

                return new DbResponse<List<Notification>> { Success = true, Data = list };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Notification>> { Success = false, Message = $"Error fetching announcements: {ex.Message}" };
            }
        }

        public async Task<DbResponse<Notification>> GetAnnouncementByIdAsync(int id)
        {
            try
            {
                var entity = await _context.Notifications.FirstOrDefaultAsync(n => n.NotificationId == id);

                if (entity == null)
                    return new DbResponse<Notification> { Success = false, Message = "Announcement not found" };

                return new DbResponse<Notification> { Success = true, Data = entity };
            }
            catch (Exception ex)
            {
                return new DbResponse<Notification> { Success = false, Message = $"Error fetching announcement: {ex.Message}" };
            }
        }

        public async Task<DbResponse<Notification>> UpdateAnnouncementAsync(int id, NotificationDto dto)
        {
            try
            {
                var existing = await _context.Notifications.FindAsync(id);
                if (existing == null)
                    return new DbResponse<Notification> { Success = false, Message = "Announcement not found" };

                existing.Title = dto.Title;
                existing.Message = dto.Message;
                //existing.CreatedById = dto.CreatedById;
                existing.IsChurchWide = dto.IsChurchWide;
                existing.MinistryId = dto.MinistryId;
                existing.NotificationTime = dto.NotificationTime;
                existing.ExpiryTime = dto.ExpiryTime;
                existing.RequiresReminder = dto.RequiresReminder;
                existing.SendSMS = dto.SendSMS;
                existing.SendEmail = dto.SendEmail;
                existing.SendPushNotification = dto.SendPushNotification;
                existing.UpdatedAt = DateTime.Now;
                existing.IsActive = dto.IsActive;
                existing.NotificationGroupId = dto.NotificationGroupId;
                existing.GrowthCenterId = dto.GrowthCenterId ?? 0;
                existing.RcpsId = dto.RcpsId ?? 0;
                existing.PushNotificationType = dto.PushNotificationType ?? "general";
                existing.DeepLinkScreen = dto.DeepLinkScreen ?? "notifications";
                existing.DeepLinkId = dto.DeepLinkId;

                // Handle special members if NotificationGroupId == 3
                if (dto.NotificationGroupId == 3 && dto.SelectedMembers != null && dto.SelectedMembers.Any())
                {
                    // Remove existing
                    var existingMembers = await _context.SpecialNotificationMembers
                        .Where(s => s.NotificationId == id)
                        .ToListAsync();
                    if (existingMembers.Any())
                    {
                        _context.SpecialNotificationMembers.RemoveRange(existingMembers);
                    }

                    // Add new
                    var specialMembers = dto.SelectedMembers.Select(memberId => new SpecialNotificationMember
                    {
                        NotificationId = id,
                        MemberId = memberId,
                        IsNotified = false,
                        CreatedAt = DateTime.Now
                    }).ToList();
                    await _context.SpecialNotificationMembers.AddRangeAsync(specialMembers);
                }

                await _context.SaveChangesAsync();

                return new DbResponse<Notification> { Success = true, Data = existing, Message = "Announcement updated successfully" };
            }
            catch (Exception ex)
            {
                return new DbResponse<Notification> { Success = false, Message = $"Error updating announcement: {ex.Message}" };
            }
        }

        public async Task<DbResponse<bool>> DeleteAnnouncementAsync(int id)
        {
            try
            {
                var existing = await _context.Notifications.FindAsync(id);
                if (existing == null)
                    return new DbResponse<bool> { Success = false, Message = "Announcement not found" };

                _context.Notifications.Remove(existing);
                await _context.SaveChangesAsync();

                return new DbResponse<bool> { Success = true, Data = true, Message = "Announcement deleted successfully" };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Message = $"Error deleting announcement: {ex.Message}" };
            }
        }

        public async Task<DbResponse<bool>> ToggleAnnouncementStatusAsync(int id, bool isActive)
        {
            try
            {
                var existing = await _context.Notifications.FindAsync(id);
                if (existing == null)
                    return new DbResponse<bool> { Success = false, Message = "Announcement not found" };

                existing.IsActive = isActive;
                existing.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return new DbResponse<bool> { Success = true, Data = true, Message = "Announcement status updated" };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Message = $"Error toggling announcement status: {ex.Message}" };
            }
        }
        public async Task<DbResponse<List<NotificationGroup>>> GetAllNotificationGroupsAsync()
        {
            try
            {
                var list = await _context.NotificationGroups
                    .OrderByDescending(g => g.CreatedAt)
                    .ToListAsync();
                return new DbResponse<List<NotificationGroup>> { Success = true, Data = list };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<NotificationGroup>> { Success = false, Message = $"Error fetching notification groups: {ex.Message}" };
            }
        }
        public async Task<DbResponse<NotificationGroup>> GetNotificationGroupByIdAsync(int id)
        {
            try
            {
                var group = await _context.NotificationGroups.FindAsync(id);
                if (group == null)
                    return new DbResponse<NotificationGroup> { Success = false, Message = "Notification Group not found" };

                return new DbResponse<NotificationGroup> { Success = true, Data = group };
            }
            catch (Exception ex)
            {
                return new DbResponse<NotificationGroup> { Success = false, Message = $"Error fetching notification group: {ex.Message}" };
            }
        }

        public async Task<DbResponse<NotificationGroup>> CreateOrUpdateNotificationGroupAsync(NotificationGroup model)
        {
            try
            {
                if (model.GroupId == 0)
                {
                    model.CreatedAt = DateTime.Now;
                    _context.NotificationGroups.Add(model);
                }
                else
                {
                    var existingGroup = await _context.NotificationGroups.FindAsync(model.GroupId);
                    if (existingGroup == null)
                        return new DbResponse<NotificationGroup> { Success = false, Message = "Notification Group not found" };

                    existingGroup.GroupName = model.GroupName;
                    existingGroup.Description = model.Description;
                    existingGroup.IsActive = model.IsActive;
                    existingGroup.UpdatedAt = DateTime.Now;
                    _context.NotificationGroups.Update(existingGroup);
                }

                await _context.SaveChangesAsync();
                return new DbResponse<NotificationGroup> { Success = true, Data = model, Message = "Notification Group saved successfully" };
            }
            catch (Exception ex)
            {
                return new DbResponse<NotificationGroup> { Success = false, Message = $"Error saving notification group: {ex.Message}" };
            }
        }

        public async Task<DbResponse<bool>> DeleteNotificationGroupAsync(int id)
        {
            try
            {
                var group = await _context.NotificationGroups.FindAsync(id);
                if (group == null)
                    return new DbResponse<bool> { Success = false, Message = "Notification Group not found" };

                _context.NotificationGroups.Remove(group);
                await _context.SaveChangesAsync();
                return new DbResponse<bool> { Success = true, Data = true, Message = "Notification Group deleted successfully" };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Message = $"Error deleting notification group: {ex.Message}" };
            }
        }
    }
}

