using ClosedXML.Excel;
using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class EventsService : IEventsService
    {
        private readonly EventsRepository _eventsRepository;
        private readonly MembersRepository _membersRepository;
        private readonly AppDbContext _context;
        private readonly SystemConfigRepository _systemConfigRepository;
        private readonly CommunicationService _communicationService;
        private readonly string folderPath;

        public EventsService(
            EventsRepository eventsRepository,
            MembersRepository membersRepository,
            AppDbContext context,
            SystemConfigRepository systemConfigRepository,
            CommunicationService communicationService)
        {
            _eventsRepository = eventsRepository;
            _membersRepository = membersRepository;
            _context = context;
            _systemConfigRepository = systemConfigRepository;
            _communicationService = communicationService;

            folderPath = SystemConfigHelper
                .GetImageBasePathAsync(_systemConfigRepository)
                .Result;
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
                    response.Message = result.Message ?? "Failed to create event";
                    return response;
                }

                if (!string.IsNullOrEmpty(dto.ImageBase64))
                {
                    var imageBytes = ImageHelper.RemoveBase64Prefix(dto.ImageBase64);
                    ImageHelper.SaveImage(imageBytes, folderPath, $"event_{result.Data.EventId}", "jpg");
                }

                response.IsSuccess = true;
                response.Code = "200";
                response.Data = result.Data;
                response.Message = "Event created successfully";
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"CreateEventAsync -> {ex}");

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

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "400";
                response.Data = result.Data;
                response.Message = result.Message ?? "Events retrieved successfully";
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

                if (!result.Success || result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Event not found";
                    return response;
                }

                result.Data.EventImage = ImageHelper.ReadImage(folderPath, $"event_{eventId}");

                response.IsSuccess = true;
                response.Code = "200";
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
                    response.Message = result.Message ?? "Update failed";
                    return response;
                }

                if (!string.IsNullOrEmpty(dto.ImageBase64))
                {
                    var imageBytes = ImageHelper.RemoveBase64Prefix(dto.ImageBase64);
                    ImageHelper.SaveImage(imageBytes, folderPath, $"event_{eventId}", "jpg");
                }

                response.IsSuccess = true;
                response.Code = "200";
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

        public async Task<ApiResponse<Event>> UpdateEventAgeGroupsAsync(int eventId, string ageGroups)
        {
            return await _eventsRepository.UpdateEventAgeGroupsAsync(eventId, ageGroups);
        }

        public async Task<ApiResponse<bool>> DeleteEventAsync(int eventId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _eventsRepository.DeleteEventAsync(eventId);

                response.IsSuccess = result.Success;
                response.Code = result.Success ? "200" : "404";
                response.Data = result.Data;
                response.Message = result.Message ?? "Delete operation completed";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<bool>> ToggleEventStatusAsync(int eventId, bool isActive)
        {
            var eventEntity = await _context.Events.FindAsync(eventId);

            if (eventEntity == null)
            {
                return new ApiResponse<bool>
                {
                    IsSuccess = false,
                    Code = "404",
                    Message = "Event not found"
                };
            }

            eventEntity.IsActive = isActive;
            eventEntity.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse<bool>
            {
                IsSuccess = true,
                Code = "200",
                Message = isActive ? "Event activated successfully." : "Event deactivated successfully.",
                Data = true
            };
        }

        // =========================
        // FIXED EXCEL UPLOAD (SAFE)
        // =========================
        public async Task<ApiResponse<EventUploadResponse>> ProcessEventExcelUploadAsync(
            IFormFile file,
            string createdBy,
            string uploadOption)
        {
            if (file == null || file.Length == 0)
            {
                return new ApiResponse<EventUploadResponse>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = "No file uploaded"
                };
            }

            var extension = Path.GetExtension(file.FileName)?.ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return new ApiResponse<EventUploadResponse>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = "Only Excel files (.xlsx, .xls) are allowed"
                };
            }

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            try
            {
                List<ExcelEventDto> excelRows;

                using (var stream = new MemoryStream(fileBytes))
                {
                    excelRows = ReadExcelEventFile(stream);
                }

                var response = new EventUploadResponse
                {
                    TotalRecords = excelRows.Count
                };

                foreach (var row in excelRows)
                {
                    try
                    {
                        if (!DateTime.TryParse(row.EventDate, out DateTime parsedDate))
                        {
                            response.FailedRecords++;
                            continue;
                        }

                        decimal price = 0;
                        decimal.TryParse(row.Price, out price);

                        DateTime? startDateTime = DateTime.TryParse(row.StartDateTime, out var sdt) ? sdt : null;
                        DateTime? endDateTime = DateTime.TryParse(row.EndDateTime, out var edt) ? edt : null;

                        bool isPaid = ConvertYesNo(row.IsPaid);

                        var eventEntity = new EventDto
                        {
                            Title = row.Title,
                            Description = row.Description,
                            EventDate = parsedDate,
                            Location = row.Location,
                            IsPaid = isPaid,
                            Price = isPaid ? price : 0,

                            IsActive = false,
                            RequireRegistration = ConvertYesNo(row.RequireRegistration),
                            AllowWalkIns = ConvertYesNo(row.AllowWalkIns),
                            StartDateTime = startDateTime,
                            EndDateTime = endDateTime
                        };

                        var result = await _eventsRepository.CreateEventAsync(eventEntity);

                        if (result.Success)
                        {
                            response.SuccessfulRecords++;
                            response.CreatedEvents.Add(result.Data);
                        }
                        else
                        {
                            response.FailedRecords++;
                        }
                    }
                    catch
                    {
                        response.FailedRecords++;
                    }
                }

                return new ApiResponse<EventUploadResponse>
                {
                    IsSuccess = true,
                    Code = "200",
                    Message = "Upload completed",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EventUploadResponse>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                };
            }
        }

        private List<ExcelEventDto> ReadExcelEventFile(Stream fileStream)
        {
            var rows = new List<ExcelEventDto>();

            using var workbook = new XLWorkbook(fileStream);
            var sheet = workbook.Worksheet(1);

            int rowNo = 2;

            foreach (var row in sheet.RowsUsed().Skip(1))
            {
                rows.Add(new ExcelEventDto
                {
                    RowNumber = rowNo++,
                    Title = GetCellValue(row.Cell(1)),
                    Description = GetCellValue(row.Cell(2)),
                    EventDate = GetCellValue(row.Cell(3)),
                    Location = GetCellValue(row.Cell(4)),
                    IsPaid = GetCellValue(row.Cell(5)),
                    Price = GetCellValue(row.Cell(6)),
                    RequireRegistration = GetCellValue(row.Cell(7)),
                    AllowWalkIns = GetCellValue(row.Cell(8)),
                    StartDateTime = GetCellValue(row.Cell(9)),
                    EndDateTime = GetCellValue(row.Cell(10))
                });
            }

            return rows;
        }

        private bool ConvertYesNo(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            value = value.ToLower().Trim();
            return value is "yes" or "y" or "true" or "1";
        }

        private string GetCellValue(IXLCell cell)
        {
            return cell?.GetValue<string>()?.Trim();
        }

        // KEEP YOUR OTHER METHODS UNCHANGED
        public async Task<ApiResponse<List<Event>>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _eventsRepository.GetEventsByDateRangeAsync(startDate, endDate);

                return new ApiResponse<List<Event>>
                {
                    IsSuccess = response.Success,
                    Message = response.Message,
                    Data = response.Data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"EventsService -> GetEventsByDateRangeAsync -> {ex}");

                return new ApiResponse<List<Event>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public Task<ApiResponse<List<EventRegistration>>> GetEventRegistrationsByEventIdAsync(int eventId)
        {
            var response = _eventsRepository.GetEventRegistrationsByEventIdAsync(eventId);
            if (!response.Result.Success)
            {
                return Task.FromResult(new ApiResponse<List<EventRegistration>>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = response.Result.Message ?? "Failed to retrieve event registrations"
                });
            }

            return Task.FromResult(new ApiResponse<List<EventRegistration>>
            {
                IsSuccess = true,
                Code = "200",
                Message = response.Result.Message ?? "Event registrations retrieved successfully",
                Data = response.Result.Data
            });
        }

        public Task<ApiResponse<List<EventRegistration>>> GetEventRegistrationsAsync()
        {
            throw new NotImplementedException();
        }

       
                public async Task<ApiResponse<List<AnnualTheme>>> GetAllAnnualThemesAsync(string? assemblyName = null)
        {
            var response = new ApiResponse<List<AnnualTheme>>();
            try
            {
                var result = await _eventsRepository.GetAllAnnualThemesAsync(assemblyName);
                if (result.Success)
                {
                    response.IsSuccess = true;
                    response.Data = result.Data;
                    response.Message = "Themes retrieved successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteAnnualThemeAsync(int id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var result = await _eventsRepository.DeleteAnnualThemeAsync(id);
                response.IsSuccess = result.Success;
                response.Data = result.Data;
                response.Message = result.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<List<MonthlyTheme>>> GetAllMonthlyThemesAsync(string? assemblyName = null)
        {
            var response = new ApiResponse<List<MonthlyTheme>>();
            try
            {
                var result = await _eventsRepository.GetAllMonthlyThemesAsync(assemblyName);
                if (result.Success)
                {
                    response.IsSuccess = true;
                    response.Data = result.Data;
                    response.Message = "Themes retrieved successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<bool>> DeleteMonthlyThemeAsync(int id)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var result = await _eventsRepository.DeleteMonthlyThemeAsync(id);
                response.IsSuccess = result.Success;
                response.Data = result.Data;
                response.Message = result.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<AnnualTheme>> GetCurrentYearThemeAsync(string? assemblyName = null)
        {
            var response = new ApiResponse<AnnualTheme>();

            try
            {
                DateTime currentYear = DateTime.Now;


                var result = await _eventsRepository.GetThemeForCurrentYearAsync(DateTime.Now, assemblyName);

                if (result == null || result.Data == null)
                {

                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "No theme found for the current year";
                    return response;
                }

                if (string.IsNullOrEmpty(assemblyName))
                {
                    result.Data.YearThemeImage = ImageHelper.ReadImage(folderPath, "theme_" + result.Data.Year);
                }
                else
                {
                    var assemblyFolder = Path.Combine(folderPath, "Assemblies", assemblyName);
                    result.Data.YearThemeImage = ImageHelper.ReadImage(assemblyFolder, "theme_" + result.Data.Year);
                }

                response.IsSuccess = true;
                response.Code = "200";
                response.Data = result.Data;
                response.Message = "Current year theme retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = $"Error fetching theme: {ex.Message}";
            }

            return response;
        }

        public Task<ApiResponse<AnnualTheme>> UpdateAnnualThemeAsync(int id, AnnualThemeDto dto, string? assemblyName = null)
        {
            var response = _eventsRepository.UpdateAnnualThemeAsync(id, dto, assemblyName);
            if (!response.Result.Success)
            {
                return Task.FromResult(new ApiResponse<AnnualTheme>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = response.Result.Message ?? "Failed to update annual theme"
                });
            }
            if (dto.ThemeImage != null) {
                var imageBytes = ImageHelper.RemoveBase64Prefix(dto.ThemeImage);
                if (string.IsNullOrEmpty(assemblyName))
                {
                    string savedImagePath = ImageHelper.SaveImage(imageBytes, folderPath, "theme_" + dto.Year, "jpg");
                    Loggers.EventLogs($"Saved global theme image to: {savedImagePath}");
                }
                else
                {
                    var assemblyFolder = Path.Combine(folderPath, "Assemblies", assemblyName);
                    if (!Directory.Exists(assemblyFolder)) Directory.CreateDirectory(assemblyFolder);
                    string savedImagePath = ImageHelper.SaveImage(imageBytes, assemblyFolder, "theme_" + dto.Year, "jpg");
                    Loggers.EventLogs($"Saved assembly theme image to: {savedImagePath}");
                }
            }

            return Task.FromResult(new ApiResponse<AnnualTheme>
            {
                IsSuccess = true,
                Code = "200",
                Message = response.Result.Message ?? "Annual theme updated successfully",
                Data = response.Result.Data
            });
        }

                public async Task<ApiResponse<MonthlyTheme>> GetCurrentMonthlyThemeAsync(string? assemblyName = null)
        {
            var response = new ApiResponse<MonthlyTheme>();

            try
            {
                var result = await _eventsRepository.GetThemeForCurrentMonthAsync(DateTime.Now, assemblyName);

                if (result == null || result.Data == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "No theme found for the current month";
                    return response;
                }

                if (string.IsNullOrEmpty(assemblyName))
                {
                    result.Data.MonthThemeImage = ImageHelper.ReadImage(folderPath, "monthlytheme_" + result.Data.Year + "_" + result.Data.Month);
                }
                else
                {
                    var assemblyFolder = Path.Combine(folderPath, "Assemblies", assemblyName);
                    result.Data.MonthThemeImage = ImageHelper.ReadImage(assemblyFolder, "monthlytheme_" + result.Data.Year + "_" + result.Data.Month);
                }

                response.IsSuccess = true;
                response.Code = "200";
                response.Data = result.Data;
                response.Message = "Current month theme retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = $"Error fetching monthly theme: {ex.Message}";
            }

            return response;
        }



        public Task<ApiResponse<MonthlyTheme>> UpdateMonthlyThemeAsync(int id, MonthlyThemeDto dto, string? assemblyName = null)
        {
            var response = _eventsRepository.UpdateMonthlyThemeAsync(id, dto, assemblyName);
            if (!response.Result.Success)
            {
                return Task.FromResult(new ApiResponse<MonthlyTheme>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = response.Result.Message ?? "Failed to update monthly theme"
                });
            }
            if (dto.ThemeImage != null) {
                var imageBytes = ImageHelper.RemoveBase64Prefix(dto.ThemeImage);
                if (string.IsNullOrEmpty(assemblyName))
                {
                    string savedImagePath = ImageHelper.SaveImage(imageBytes, folderPath, "monthlytheme_" + dto.Year + "_" + dto.Month, "jpg");
                    Loggers.EventLogs($"Saved global monthly theme image to: {savedImagePath}");
                }
                else
                {
                    var assemblyFolder = Path.Combine(folderPath, "Assemblies", assemblyName);
                    if (!Directory.Exists(assemblyFolder)) Directory.CreateDirectory(assemblyFolder);
                    string savedImagePath = ImageHelper.SaveImage(imageBytes, assemblyFolder, "monthlytheme_" + dto.Year + "_" + dto.Month, "jpg");
                    Loggers.EventLogs($"Saved assembly monthly theme image to: {savedImagePath}");
                }
            }

            return Task.FromResult(new ApiResponse<MonthlyTheme>
            {
                IsSuccess = true,
                Code = "200",
                Message = response.Result.Message ?? "Monthly theme updated successfully",
                Data = response.Result.Data
            });
        }


        public Task<ApiResponse<List<Event>>> GetUpcomingEventsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<string>> SendCollectionReminderAsync(int id)
        {
            var response = new ApiResponse<string>();
            try
            {
                var reg = await _eventsRepository.GetEventRegistrationByIdAsync(id);

                if (reg == null)
                {
                    Loggers.EventLogs($"SendCollectionReminder: No record found for Registration ID: {id}");
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Registration not found.";
                    return response;
                }

                if (reg.PaymentStatusId == 4)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "User has already paid.";
                    return response;
                }

                string phone = "";
                string email = "";
                string name = "Guest";

                if (reg.MemberId != 0)
                {
                    var memberResponse = await _membersRepository.GetMemberByIdAsync(reg.MemberId);
                    var member = memberResponse.Data;
                    phone = member?.Phone;
                    email = member?.Email;
                    name = member?.FirstName ?? "Guest";
                }
                else
                {
                    phone = reg.GuestPhone;
                    email = reg.GuestEmail;
                    name = !string.IsNullOrWhiteSpace(reg.GuestName) ? reg.GuestName : "Guest";
                }

                string eventName = reg.Event?.Title ?? "the upcoming event";

                if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(email))
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "No contact information available for this user.";
                    return response;
                }

                string message =
                    $"Hello {name}, we hope you're doing well. This is a gentle reminder that your Collection for {eventName} is still pending." +
                    $"\nPlease click the link to complete your Collection." +
                    $"\nhttps://portal.gospelcentresinternational.com/Register/Event/{reg.EventId}" +
                    $"\nThank you and God bless!";

                bool sent = false;

                if (!string.IsNullOrEmpty(phone))
                {
                    await _communicationService.SendSmsAsync(phone, message);
                    sent = true;
                }

                if (!string.IsNullOrEmpty(email) && !sent)
                {
                    await _communicationService.SendEmailAsync(email, $"Collection Reminder: {eventName}", message);
                }

                response.IsSuccess = true;
                response.Code = "200";
                response.Message = "Collection reminder sent successfully.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<string>> SendBulkCollectionRemindersAsync(int eventId)
        {
            var response = new ApiResponse<string>();
            try
            {
                Loggers.EventLogs($"SendBulkCollectionReminders: Starting to send Collection reminders for Event ID: {eventId}");
                
                var registrations = await _eventsRepository.GetEventRegistrationsForCollectionReminderAsync(eventId);
                    
                Loggers.EventLogs($"SendBulkCollectionReminders: Found {registrations.Count} registrations for Event ID: {eventId} with unpaid status.");

                if (!registrations.Any())
                {
                    Loggers.EventLogs($"SendBulkCollectionReminders: No record found for Event ID: {eventId}");
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "No unpaid registrations found for this event.";
                    return response;
                }

                int count = 0;

                foreach (var item in registrations)
                {
                    string phone = "";
                    string email = "";
                    string name = "Guest";

                    if (item.MemberId != 0)
                    {
                        var memberResponse = await _membersRepository.GetMemberByIdAsync(item.MemberId);
                        var member = memberResponse.Data;
                        phone = member?.Phone;
                        email = member?.Email;
                        name = member?.FirstName ?? "Guest";
                    }
                    else
                    {
                        phone = item.GuestPhone;
                        email = item.GuestEmail;
                        name = !string.IsNullOrWhiteSpace(item.GuestName) ? item.GuestName : "Guest";
                    }

                    string eventName = item.Event?.Title ?? "the upcoming event";

                    if (!string.IsNullOrEmpty(phone) || !string.IsNullOrEmpty(email))
                    {
                        string message =
                            $"Hello {name}, we hope you're doing well. This is a gentle reminder that your Collection for {eventName} is still pending." +
                            $"\nPlease click the link to complete your Collection." +
                            $"\nhttps://portal.gospelcentresinternational.com/Register/Event/{item.EventId}" +
                            $"\nThank you and God bless!";

                        if (!string.IsNullOrEmpty(phone))
                        {
                            await _communicationService.SendSmsAsync(phone, message);
                        }
                        else
                        {
                            await _communicationService.SendEmailAsync(email, $"Collection Reminder: {eventName}", message);
                        }

                        count++;
                    }
                }

                response.IsSuccess = true;
                response.Code = "200";
                response.Message = $"Successfully sent Collection reminders to {count} registrants.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<string>> SendAttendanceReminderAsync(int id)
        {
            var response = new ApiResponse<string>();
            try
            {
                var reg = await _eventsRepository.GetEventRegistrationByIdAsync(id);

                if (reg == null)
                {
                    Loggers.EventLogs($"SendAttendanceReminder: No record found for Registration ID: {id}");
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Registration not found.";
                    return response;
                }

                string phone = "";
                string email = "";
                string name = "Guest";

                if (reg.MemberId != 0)
                {
                    var memberResponse = await _membersRepository.GetMemberByIdAsync(reg.MemberId);
                    var member = memberResponse.Data;
                    phone = member?.Phone;
                    email = member?.Email;
                    name = member?.FirstName ?? "Guest";
                }
                else
                {
                    phone = reg.GuestPhone;
                    email = reg.GuestEmail;
                    name = !string.IsNullOrWhiteSpace(reg.GuestName) ? reg.GuestName : "Guest";
                }

                string eventName = reg.Event?.Title ?? "the upcoming event";

                if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(email))
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "No contact information available for this user.";
                    return response;
                }

                string message =
                    $"Hello {name}, we hope you're doing well. This is a Gentle reminder about your upcoming attendance for {eventName}. We are looking forward to welcoming you and sharing this special time together. We can't wait to see you. Thank you and God bless!";

                bool sent = false;

                if (!string.IsNullOrEmpty(phone))
                {
                    await _communicationService.SendSmsAsync(phone, message);
                    sent = true;
                }

                if (!string.IsNullOrEmpty(email) && !sent)
                {
                    await _communicationService.SendEmailAsync(email, $"Attendance Reminder: {eventName}", message);
                }

                response.IsSuccess = true;
                response.Code = "200";
                response.Message = "Attendance reminder sent successfully.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<string>> SendBulkAttendanceRemindersAsync(int eventId)
        {
            var response = new ApiResponse<string>();
            try
            {
                var registrations = await _eventsRepository.GetEventRegistrationsForAttendanceReminderAsync(eventId);

                if (!registrations.Any())
                {
                    Loggers.EventLogs($"SendBulkAttendanceReminders: No record found for Event ID: {eventId}");
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "No registrations pending attendance found for this event.";
                    return response;
                }

                int count = 0;

                foreach (var item in registrations)
                {
                    string phone = "";
                    string email = "";
                    string name = "Guest";

                    if (item.MemberId != 0)
                    {
                        var memberResponse = await _membersRepository.GetMemberByIdAsync(item.MemberId);
                        var member = memberResponse.Data;
                        phone = member?.Phone;
                        email = member?.Email;
                        name = member?.FirstName ?? "Guest";
                    }
                    else
                    {
                        phone = item.GuestPhone;
                        email = item.GuestEmail;
                        name = !string.IsNullOrWhiteSpace(item.GuestName) ? item.GuestName : "Guest";
                    }

                    string eventName = item.Event?.Title ?? "the upcoming event";

                    if (!string.IsNullOrEmpty(phone) || !string.IsNullOrEmpty(email))
                    {
                        string message =
                            $"Hello {name}, we hope you're doing well. This is a Gentle reminder about your upcoming attendance for {eventName}. We are looking forward to welcoming you and sharing this special time together. We can't wait to see you. Thank you and God bless!";

                        if (!string.IsNullOrEmpty(phone))
                        {
                            await _communicationService.SendSmsAsync(phone, message);
                        }
                        else
                        {
                            await _communicationService.SendEmailAsync(email, $"Attendance Reminder: {eventName}", message);
                        }

                        count++;
                    }
                }

                response.IsSuccess = true;
                response.Code = "200";
                response.Message = $"Successfully sent attendance reminders to {count} registrants.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<ApiResponse<object>> CheckEventRegistrationAsync(string phone, int eventId)
        {
            var response = new ApiResponse<object>();
            try
            {
                var registrationsQuery = await _eventsRepository.GetEventRegistrationsByPhoneAndEventAsync(phone, eventId);

                if (!registrationsQuery.Any())
                {
                    response.IsSuccess = true;
                    response.Data = new { isRegistered = false };
                    return response;
                }

                var records = registrationsQuery.Select(r => new {
                    CollectionstatusId = r.PaymentStatusId,
                    registrationId = r.RegistrationId,
                    guestName = r.GuestName ?? (r.Member != null ? $"{r.Member.FirstName} {r.Member.OtherNames}".Trim() : "N/A")
                }).ToList();

                response.IsSuccess = true;
                response.Data = new { 
                    isRegistered = true, 
                    records = records 
                };
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                Loggers.DoLogs($"Error checking event registration: {ex}");
            }
            return response;
        }

        public async Task<ApiResponse<int>> CheckCollectionstatusAsync(int registrationId)
        {
            var response = new ApiResponse<int>();
            try
            {
                var registration = await _eventsRepository.GetEventRegistrationByIdAsync(registrationId);

                if (registration == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Registration not found";
                    return response;
                }

                response.IsSuccess = true;
                response.Data = registration.PaymentStatusId;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                Loggers.DoLogs($"Error checking Collection status: {ex}");
            }
            return response;
        }

        public async Task<ApiResponse<bool>> UsherSubmitRegistrationAsync(GCI_Admin.Controllers.UsherRegistrationDto dto)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var eventItem = await _eventsRepository.GetEventByIdAsync(dto.eventId);
                if (eventItem.Data == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Event not found.";
                    return response;
                }

                var existingMember = await _eventsRepository.GetMemberByPhoneOrEmailAsync(dto.guestPhone, dto.guestEmail);
                var memberId = existingMember != null ? existingMember.Id : 0;

                EventRegistration existingRegistration = null;
                if (memberId != 0)
                {
                    existingRegistration = await _eventsRepository.GetEventRegistrationByMemberAsync(dto.eventId, memberId);
                }
                var existingGuestRegistration = await _eventsRepository.GetGuestEventRegistrationAsync(dto.eventId, dto.guestPhone, dto.guestName);

                int newCollectionstatusId = dto.isPaid ? 4 : 2; // 4 = Paid, 2 = Pending/Not Paid
                if ((existingRegistration != null && existingRegistration.PaymentStatusId == 4) || 
                    (existingGuestRegistration != null && existingGuestRegistration.PaymentStatusId == 4))
                {
                    response.IsSuccess = false;
                    response.Message = "Guest is already registered and paid.";
                    return response;
                }
                else if ((existingRegistration != null && existingRegistration.PaymentStatusId != 4) || 
                         (existingGuestRegistration != null && existingGuestRegistration.PaymentStatusId != 4))
                {
                    var regToUpdate = existingRegistration ?? existingGuestRegistration;
                    regToUpdate.PaymentStatusId = newCollectionstatusId;
                    regToUpdate.RegistrationDate = DateTime.UtcNow;
                    regToUpdate.AmountPaid = dto.amountPaid;
                    
                    await _eventsRepository.UpdateEventRegistrationAsync(regToUpdate);
                    
                    response.IsSuccess = true;
                    response.Message = dto.isPaid ? "Registration updated to paid." : "Registration updated.";
                    return response;
                }

                var registration = new EventRegistration
                {
                    EventId = dto.eventId,
                    MemberId = memberId,
                    GuestName = dto.guestName,
                    GuestEmail = dto.guestEmail,
                    GuestPhone = dto.guestPhone,
                    GuestAssembly = dto.guestAssembly,
                    GuestAgeGroup = dto.guestAgeGroup,
                    PaymentStatusId = newCollectionstatusId,
                    AmountPaid = dto.amountPaid,
                    RegistrationDate = DateTime.UtcNow,
                    HasAttended = false
                };

                await _eventsRepository.AddEventRegistrationAsync(registration);

                response.IsSuccess = true;
                response.Message = "Registration successful.";
                Loggers.EventLogs($"Registration added for Event ID {dto.eventId} via UsherSubmit.");
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
                Loggers.DoLogs($"Error in UsherSubmitRegistrationAsync: {ex}");
            }
            return response;
        }

        public async Task<ApiResponse<string>> SubmitGroupRestAsync(GCI_Admin.Controllers.SubmitGroupRestDto dto)
        {
            var response = new ApiResponse<string>();
            try
            {
                string groupId = Guid.NewGuid().ToString("N");
                if (dto.primaryRegistrationId > 0)
                {
                    var primaryRegistration = await _eventsRepository.GetEventRegistrationByIdAsync(dto.primaryRegistrationId);
                    if (primaryRegistration != null)
                    {
                        primaryRegistration.GroupId = groupId;
                        await _eventsRepository.UpdateEventRegistrationAsync(primaryRegistration);
                    }
                }
                else if (dto.guests != null && dto.guests.Any())
                {
                    var firstGuest = dto.guests.First();
                    var phoneRegistrations = await _eventsRepository.GetEventRegistrationsByPhoneAndEventAsync(firstGuest.guestPhone, firstGuest.eventId);
                    var primaryRegistration = phoneRegistrations.OrderByDescending(r => r.RegistrationDate).FirstOrDefault();
                    if (primaryRegistration != null && string.IsNullOrEmpty(primaryRegistration.GroupId))
                    {
                        primaryRegistration.GroupId = groupId;
                        await _eventsRepository.UpdateEventRegistrationAsync(primaryRegistration);
                    }
                }

                foreach (var guest in dto.guests)
                {
                    var registration = new EventRegistration
                    {
                        EventId = guest.eventId,
                        MemberId = 0,
                        GuestName = guest.guestName,
                        GuestEmail = guest.guestEmail,
                        GuestPhone = PhoneHelper.NormalizeKenyanPhoneOrEmail(guest.guestPhone),
                        GuestAssembly = guest.guestAssembly,
                        GuestAgeGroup = guest.guestAgeGroup,
                        PaymentStatusId = guest.isPaid ? 4 : 2,
                        AmountPaid = guest.amountPaid,
                        RegistrationDate = DateTime.UtcNow,
                        HasAttended = false,
                        GroupId = groupId
                    };
                    await _eventsRepository.AddEventRegistrationAsync(registration);
                }

                response.IsSuccess = true;
                response.Message = "Group submitted successfully.";
                response.Data = groupId;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "An error occurred: " + ex.Message;
                Loggers.DoLogs($"Error in SubmitGroupRestAsync: {ex}");
            }
            return response;
        }
    }
}

