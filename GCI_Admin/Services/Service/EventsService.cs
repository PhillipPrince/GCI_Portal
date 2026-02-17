using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class EventsService : IEventsService
    {
        private readonly EventsRepository _eventsRepository;
        private readonly MembersRepository _membersRepository;
        private readonly AppDbContext _context;

        public EventsService(EventsRepository eventsRepository, MembersRepository membersRepository, AppDbContext context)
        {
            _eventsRepository = eventsRepository;
            _membersRepository = membersRepository;
            _context = context;
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
        public async Task<ApiResponse<List<EventRegistration>>> GetEventRegistrationsAsync()
        {
            var response = new ApiResponse<List<EventRegistration>>();
            try
            {
                var result = await _eventsRepository.GetEventRegistrationsAsync();
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = "Failed to retrieve event registrations";
                    return response;
                }
                else
                {
                    foreach (var registration in result.Data)
                    {
                        var eventResult = await _eventsRepository.GetEventByIdAsync(registration.EventId);
                        var userResult = await _membersRepository.GetMemberByIdAsync(registration.UserId);
                        registration.Event = eventResult.Data;
                        registration.User = userResult.Data;
                    }
                    response.Data = result.Data;
                    response.Message = "Event registrations retrieved successfully";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<EventUploadResponse>> ProcessEventExcelUploadAsync(IFormFile file, string createdBy, string uploadOption)
        {
            // 1. Validation
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

            if (file.Length > 10 * 1024 * 1024)
            {
                return new ApiResponse<EventUploadResponse>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = "File size cannot exceed 10MB"
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

                if (excelRows == null || excelRows.Count == 0)
                {
                    return new ApiResponse<EventUploadResponse>
                    {
                        IsSuccess = false,
                        Code = "400",
                        Message = "No valid data found in Excel file"
                    };
                }

                var response = new EventUploadResponse
                {
                    TotalRecords = excelRows.Count
                };

                foreach (var row in excelRows)
                {
                    try
                    {
                        // Required field validation
                        if (string.IsNullOrWhiteSpace(row.Title) ||
                            string.IsNullOrWhiteSpace(row.EventDate) ||
                            string.IsNullOrWhiteSpace(row.Location))
                        {
                            response.FailedRecords++;
                            response.ErrorMessages.Add($"Row {row.RowNumber}: Missing required fields.");
                            continue;
                        }

                        if (!DateTime.TryParse(row.EventDate, out DateTime parsedDate))
                        {
                            response.FailedRecords++;
                            response.ErrorMessages.Add($"Row {row.RowNumber}: Invalid Event Date format.");
                            continue;
                        }

                        decimal price = 0;
                        if (!string.IsNullOrWhiteSpace(row.Price))
                        {
                            if (!decimal.TryParse(row.Price, out price))
                            {
                                response.FailedRecords++;
                                response.ErrorMessages.Add($"Row {row.RowNumber}: Invalid price format.");
                                continue;
                            }
                        }

                        bool isPaid = ConvertYesNo(row.IsPaid);

                        var eventEntity = new EventDto
                        {
                            Title = row.Title,
                            Description = row.Description,
                            EventDate = parsedDate,
                            Location = row.Location,
                            IsPaid = isPaid,
                            Price = isPaid ? price : 0,
                            
                        };
                        var result = await _eventsRepository.CreateEventAsync(eventEntity);

                        if (result.Success)
                        {
                            response.SuccessfulRecords++;
                            response.CreatedEvents.Add(result.Data);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.FailedRecords++;
                        response.ErrorMessages.Add($"Row {row.RowNumber}: Database error - {ex.Message}");
                    }
                }

                return new ApiResponse<EventUploadResponse>
                {
                    IsSuccess = true,
                    Code = "200",
                    Message = $"Upload completed. Total: {response.TotalRecords}, Success: {response.SuccessfulRecords}, Failed: {response.FailedRecords}",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<EventUploadResponse>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = $"Error processing Excel file: {ex.Message}"
                };
            }
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


        private List<ExcelEventDto> ReadExcelEventFile(Stream fileStream)
        {
            var rows = new List<ExcelEventDto>();

            try
            {
                using var workbook = new XLWorkbook(fileStream);
                var worksheet = workbook.Worksheet(1);

                if (worksheet == null)
                    return rows;

                var usedRows = worksheet.RowsUsed();
                if (usedRows == null)
                    return rows;

                var dataRows = usedRows.Skip(1); // Skip header
                int rowNumber = 2;

                foreach (var row in dataRows)
                {
                    var excelRow = new ExcelEventDto
                    {
                        RowNumber = rowNumber,
                        Title = GetCellValue(row.Cell(1)),
                        Description = GetCellValue(row.Cell(2)),
                        EventDate = GetCellValue(row.Cell(3)),
                        Location = GetCellValue(row.Cell(4)),
                        IsPaid = GetCellValue(row.Cell(5)),
                        Price = GetCellValue(row.Cell(6))
                    };

                    if (!IsEmptyEventRow(excelRow))
                        rows.Add(excelRow);

                    rowNumber++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading Excel file: " + ex);
                throw;
            }

            return rows;
        }

        private bool ConvertYesNo(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            value = value.Trim().ToLower();
            return value == "yes" || value == "true" || value == "1";
        }

        private bool IsEmptyEventRow(ExcelEventDto row)
        {
            return string.IsNullOrWhiteSpace(row.Title) &&
                   string.IsNullOrWhiteSpace(row.Description) &&
                   string.IsNullOrWhiteSpace(row.EventDate) &&
                   string.IsNullOrWhiteSpace(row.Location);
        }

        private string GetCellValue(IXLCell cell)
        {
            return cell?.GetValue<string>()?.Trim();
        }



    }
}
