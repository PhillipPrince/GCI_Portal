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
        private readonly string folderPath;

        public EventsService(
            EventsRepository eventsRepository,
            MembersRepository membersRepository,
            AppDbContext context,
            SystemConfigRepository systemConfigRepository)
        {
            _eventsRepository = eventsRepository;
            _membersRepository = membersRepository;
            _context = context;
            _systemConfigRepository = systemConfigRepository;

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

       
        public async Task<ApiResponse<AnnualTheme>> GetCurrentYearThemeAsync()
        {
            var response = new ApiResponse<AnnualTheme>();

            try
            {
                DateTime currentYear = DateTime.Now;


                var result = await _eventsRepository.GetThemeForCurrentYearAsync(currentYear);

                if (result == null || result.Data == null)
                {

                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "No theme found for the current year";
                    return response;
                }

                result.Data.YearThemeImage = ImageHelper.ReadImage(folderPath, currentYear.Year.ToString());

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

        public Task<ApiResponse<AnnualTheme>> UpdateAnnualThemeAsync(int id, AnnualThemeDto dto)
        {
            var response = _eventsRepository.UpdateAnnualThemeAsync(id, dto);
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

                string savedImagePath = ImageHelper.SaveImage(ImageHelper.RemoveBase64Prefix(dto.ThemeImage), folderPath, dto.Year.ToString(), "jpg");
                Loggers.EventLogs($"Saved theme image to: {savedImagePath}");
            }

            return Task.FromResult(new ApiResponse<AnnualTheme>
            {
                IsSuccess = true,
                Code = "200",
                Message = response.Result.Message ?? "Annual theme updated successfully",
                Data = response.Result.Data
            });
        }

        public Task<ApiResponse<List<Event>>> GetUpcomingEventsAsync()
        {
            throw new NotImplementedException();
        }
    }
}