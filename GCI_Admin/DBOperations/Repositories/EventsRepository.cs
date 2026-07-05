using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class EventsRepository
    {
        private readonly AppDbContext _context;

        public EventsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse<Event>> CreateEventAsync(EventDto dto)
        {
            try
            {
                var newEvent = new Event
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    EventDate = dto.EventDate,
                    Location = dto.Location,
                    IsPaid = dto.IsPaid,
                    Price = dto.IsPaid ? dto.Price : null,

                    IsActive = dto.IsActive,
                    RequireRegistration = dto.RequireRegistration,
                    AllowWalkIns = dto.AllowWalkIns,
                    StartDateTime = dto.StartDateTime,
                    EndDateTime = dto.EndDateTime,
                    GroupId = dto.GroupId ?? 1,
                    MinistryId = dto.MinistryId,

                    CreatedAt = DateTime.Now,
                    QrCode = Guid.NewGuid().ToString("N")
                };

                _context.Events.Add(newEvent);

                await _context.SaveChangesAsync();

                return new DbResponse<Event>
                {
                    Success = true,
                    Message = "Event created successfully",
                    Data = newEvent
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in CreateEventAsync: {ex}");

                return new DbResponse<Event>
                {
                    Success = false,
                    Message = $"Error creating event: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<Event>>> GetAllEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .OrderBy(e => e.EventDate)
                    .ToListAsync();

                return new DbResponse<List<Event>>
                {
                    Success = true,
                    Data = events
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Event>>
                {
                    Success = false,
                    Message = $"Error fetching events: {ex.Message}"
                };
            }
        }

        // ✅ GET EVENT BY ID
        public async Task<DbResponse<Event>> GetEventByIdAsync(int eventId)
        {
            try
            {
                var eventItem = await _context.Events
                    .FirstOrDefaultAsync(e => e.EventId == eventId);

                if (eventItem == null)
                {
                    return new DbResponse<Event>
                    {
                        Success = false,
                        Message = "Event not found"
                    };
                }

                return new DbResponse<Event>
                {
                    Success = true,
                    Data = eventItem
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Event>
                {
                    Success = false,
                    Message = $"Error fetching event: {ex.Message}"
                };
            }
        }

        // ✅ UPDATE EVENT
        public async Task<DbResponse<Event>> UpdateEventAsync(int eventId, EventDto dto)
        {
            try
            {
                var existingEvent = await _context.Events.FindAsync(eventId);

                if (existingEvent == null)
                {
                    return new DbResponse<Event>
                    {
                        Success = false,
                        Message = "Event not found"
                    };
                }

                existingEvent.Title = dto.Title;
                existingEvent.Description = dto.Description;
                existingEvent.EventDate = dto.EventDate;
                existingEvent.Location = dto.Location;
                existingEvent.IsPaid = dto.IsPaid;
                existingEvent.Price = dto.IsPaid ? dto.Price : null;
                existingEvent.IsActive = dto.IsActive;
                existingEvent.RequireRegistration = dto.RequireRegistration;
                existingEvent.AllowWalkIns = dto.AllowWalkIns;
                existingEvent.StartDateTime = dto.StartDateTime;
                existingEvent.EndDateTime = dto.EndDateTime;
                existingEvent.GroupId = dto.GroupId ?? 1;
                existingEvent.MinistryId = dto.MinistryId;
                existingEvent.UpdatedAt = DateTime.Now;

                if (string.IsNullOrEmpty(existingEvent.QrCode))
                {
                    existingEvent.QrCode = Guid.NewGuid().ToString("N");
                }

                await _context.SaveChangesAsync();

                return new DbResponse<Event>
                {
                    Success = true,
                    Message = "Event updated successfully",
                    Data = existingEvent
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Event>
                {
                    Success = false,
                    Message = $"Error updating event: {ex.Message}"
                };
            }
        }

        // ✅ DELETE EVENT
        public async Task<DbResponse<bool>> DeleteEventAsync(int eventId)
        {
            try
            {
                var eventItem = await _context.Events.FindAsync(eventId);

                if (eventItem == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Event not found"
                    };
                }

                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Event deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting event: {ex.Message}"
                };
            }
        }
        public async Task<DbResponse<AnnualEventCalendar>> CreateAnnualEventAsync(AnnualEventCalendarDto dto)
        {
            try
            {
                var newEvent = new AnnualEventCalendar
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    EventStartDate = dto.EventStartDate,
                    EventEndDate = dto.EventEndDate,
                    Year = dto.Year,
                    Location = dto.Location,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now
                };

                _context.AnnualEventCalendars.Add(newEvent);
                await _context.SaveChangesAsync();

                return new DbResponse<AnnualEventCalendar>
                {
                    Success = true,
                    Message = "Annual event created successfully",
                    Data = newEvent
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in CreateAnnualEventAsync: {ex}");
                return new DbResponse<AnnualEventCalendar>
                {
                    Success = false,
                    Message = $"Error creating annual event: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<AnnualEventCalendar>>> GetAllAnnualEventsAsync()
        {
            try
            {
                var events = await _context.AnnualEventCalendars
                    .OrderByDescending(e => e.EventStartDate)
                    .ToListAsync();

                return new DbResponse<List<AnnualEventCalendar>>
                {
                    Success = true,
                    Data = events
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<AnnualEventCalendar>>
                {
                    Success = false,
                    Message = $"Error fetching annual events: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<AnnualEventCalendar>> UpdateAnnualEventAsync(int calendarEventId, AnnualEventCalendarDto dto)
        {
            try
            {
                var existingEvent = await _context.AnnualEventCalendars.FindAsync(calendarEventId);

                if (existingEvent == null)
                {
                    return new DbResponse<AnnualEventCalendar>
                    {
                        Success = false,
                        Message = "Annual event not found"
                    };
                }

                existingEvent.Title = dto.Title;
                existingEvent.Description = dto.Description;
                existingEvent.EventStartDate = dto.EventStartDate;
                existingEvent.EventEndDate = dto.EventEndDate;
                existingEvent.Year = dto.Year;
                existingEvent.Location = dto.Location;
                existingEvent.IsActive = dto.IsActive;
                existingEvent.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<AnnualEventCalendar>
                {
                    Success = true,
                    Message = "Annual event updated successfully",
                    Data = existingEvent
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in UpdateAnnualEventAsync: {ex}");
                return new DbResponse<AnnualEventCalendar>
                {
                    Success = false,
                    Message = $"Error updating annual event: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<bool>> DeleteAnnualEventAsync(int calendarEventId)
        {
            try
            {
                var eventItem = await _context.AnnualEventCalendars.FindAsync(calendarEventId);

                if (eventItem == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Annual event not found"
                    };
                }

                eventItem.IsActive = false;
                eventItem.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Annual event deactivated successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting annual event: {ex.Message}"
                };
            }
        }
        //public async Task<DbResponse<List<EventRegistration>>> GetEventRegistrationsAsync()
        //{
        //    try
        //    {
        //        var today = DateTime.Today;
        //        var upcomingEventIds = await _context.Events
        //            .Where(e => e.EventDate >= today)
        //            .Select(e => e.EventId)
        //            .ToListAsync();
        //        if (!upcomingEventIds.Any())
        //        {
        //            return new DbResponse<List<EventRegistration>>
        //            {
        //                Success = true,
        //                Message =" No upcoming events found",
        //                Data = new List<EventRegistration>()
        //            };
        //        }

        //        var registrations = await _context.EventRegistrations
        //            .Where(r => upcomingEventIds.Contains(r.EventId))
        //            .ToListAsync();

        //        return new DbResponse<List<EventRegistration>>
        //        {
        //            Success = true,
        //            Data = registrations
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DbResponse<List<EventRegistration>>
        //        {
        //            Success = false,
        //            Message = $"Error fetching event registrations: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<DbResponse<List<EventRegistration>>> GetEventRegistrationsAsync()
        //{
        //    try
        //    {
        //        var data = await _context.EventRegistrations
        //            //.Include(r => r.Event)
        //            //.Include(r => r.User)
        //            .OrderByDescending(r => r.RegistrationDate)
        //            .ToListAsync();

        //        return new DbResponse<List<EventRegistration>>
        //        {
        //            Success = true,
        //            Data = data
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DbResponse<List<EventRegistration>>
        //        {
        //            Success = false,
        //            Message = $"Error fetching event registrations: {ex.Message}"
        //        };
        //    }
        //}
        public async Task<DbResponse<List<EventRegistration>>> GetEventRegistrationsAsync()
        {
            try
            {
                // Ensure _context is not disposed
                if (_context == null)
                {
                    return new DbResponse<List<EventRegistration>>
                    {
                        Success = false,
                        Message = "Database context is not initialized"
                    };
                }

                var eventRegistrations = await _context.EventRegistrations
    .AsNoTracking()
    .Where(r => r.Event.IsActive)  
    .OrderByDescending(r => r.RegistrationDate)
    .Include(r => r.Member)
    .Include(r => r.Event)
    .ToListAsync();
                return new DbResponse<List<EventRegistration>>
                {
                    Success = true,
                    Data = eventRegistrations
                };
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("closed"))
            {
                // Specific handling for closed reader/connection
                return new DbResponse<List<EventRegistration>>
                {
                    Success = false,
                    Message = "Database connection was closed. Please try again."
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<EventRegistration>>
                {
                    Success = false,
                    Message = $"Error fetching event registrations: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<AnnualTheme>> GetThemeForCurrentYearAsync(DateTime currentYear, string? assemblyName = null)
        {
            try
            {
                var query = _context.AnnualThemes
                    .Where(t => t.IsActive && t.Year == currentYear.Year);

                if (!string.IsNullOrEmpty(assemblyName))
                {
                    query = query.Where(t => t.Assembly == assemblyName);
                }
                else
                {
                    query = query.Where(t => string.IsNullOrEmpty(t.Assembly));
                }

                var theme = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefaultAsync();

                if (theme == null)
                {
                    return new DbResponse<AnnualTheme>
                    {
                        Success = false,
                        Message = "No active theme found for the current year."
                    };
                }

                return new DbResponse<AnnualTheme>
                {
                    Success = true,
                    Data = theme
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<AnnualTheme>
                {
                    Success = false,
                    Message = $"Error fetching theme for current year: {ex.Message}"
                };
            }
        }

                public async Task<DbResponse<List<AnnualTheme>>> GetAllAnnualThemesAsync(string? assemblyName = null)
        {
            try
            {
                var query = _context.AnnualThemes.AsQueryable();
                if (!string.IsNullOrEmpty(assemblyName))
                    query = query.Where(t => t.Assembly == assemblyName);
                
                var themes = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
                return new DbResponse<List<AnnualTheme>> { Success = true, Data = themes };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<AnnualTheme>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<Event>> UpdateEventAgeGroupsAsync(int eventId, string ageGroups)
        {
            var response = new ApiResponse<Event>();
            try
            {
                var existingEvent = await _context.Events.FindAsync(eventId);
                if (existingEvent == null)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = "Event not found";
                    return response;
                }

                existingEvent.AllowedAgeGroups = ageGroups;
                existingEvent.UpdatedAt = DateTime.UtcNow;

                _context.Events.Update(existingEvent);
                await _context.SaveChangesAsync();

                response.IsSuccess = true;
                response.Code = "200";
                response.Message = "Age groups updated successfully";
                response.Data = existingEvent;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<DbResponse<bool>> DeleteAnnualThemeAsync(int id)
        {
            try
            {
                var theme = await _context.AnnualThemes.FindAsync(id);
                if (theme != null)
                {
                    _context.AnnualThemes.Remove(theme);
                    await _context.SaveChangesAsync();
                    return new DbResponse<bool> { Success = true, Data = true, Message = "Deleted successfully" };
                }
                return new DbResponse<bool> { Success = false, Message = "Not found" };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<DbResponse<List<MonthlyTheme>>> GetAllMonthlyThemesAsync(string? assemblyName = null)
        {
            try
            {
                var query = _context.MonthlyThemes.AsQueryable();
                if (!string.IsNullOrEmpty(assemblyName))
                    query = query.Where(t => t.Assembly == assemblyName);
                
                var themes = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
                return new DbResponse<List<MonthlyTheme>> { Success = true, Data = themes };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<MonthlyTheme>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<DbResponse<bool>> DeleteMonthlyThemeAsync(int id)
        {
            try
            {
                var theme = await _context.MonthlyThemes.FindAsync(id);
                if (theme != null)
                {
                    _context.MonthlyThemes.Remove(theme);
                    await _context.SaveChangesAsync();
                    return new DbResponse<bool> { Success = true, Data = true, Message = "Deleted successfully" };
                }
                return new DbResponse<bool> { Success = false, Message = "Not found" };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<DbResponse<AnnualTheme>> UpdateAnnualThemeAsync(int id, AnnualThemeDto dto, string? assemblyName = null)
        {
            try
            {
                AnnualTheme existing = null;

                if (id > 0)
                {
                    existing = await _context.AnnualThemes.FindAsync(id);
                }
                else
                {
                    var query = _context.AnnualThemes.Where(t => t.Year == dto.Year);
                    if (!string.IsNullOrEmpty(assemblyName))
                        query = query.Where(t => t.Assembly == assemblyName);
                    else
                        query = query.Where(t => string.IsNullOrEmpty(t.Assembly));

                    existing = await query.FirstOrDefaultAsync();
                }

                
                if (existing == null || existing.Year != dto.Year)
                {
                    var newTheme = new AnnualTheme
                    {
                        Theme = dto.Theme,
                        Verse = dto.Verse,
                        Description = dto.Description,
                        Year = dto.Year,
                        Assembly = assemblyName,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.AnnualThemes.Add(newTheme);
                    await _context.SaveChangesAsync();

                    Loggers.DoLogs($"Created new annual theme: {newTheme.Theme} for year {newTheme.Year}");

                    return new DbResponse<AnnualTheme>
                    {
                        Success = true,
                        Message = "Theme created successfully",
                        Data = newTheme
                    };
                }

                existing.Theme = dto.Theme;
                existing.Verse = dto.Verse;
                existing.Description = dto.Description;
                existing.IsActive = true;
                existing.UpdatedAt = DateTime.Now;

                Loggers.DoLogs($"Updated annual theme: {existing.Theme} for year {existing.Year}");

                await _context.SaveChangesAsync();

                return new DbResponse<AnnualTheme>
                {
                    Success = true,
                    Message = "Theme updated successfully",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"UpdateAnnualThemeAsync Error: {ex}");

                return new DbResponse<AnnualTheme>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

                public async Task<DbResponse<MonthlyTheme>> GetThemeForCurrentMonthAsync(DateTime currentDate, string? assemblyName = null)
        {
            try
            {
                var query = _context.MonthlyThemes
                    .Where(t => t.IsActive && t.Year == currentDate.Year && t.Month == currentDate.Month);

                if (!string.IsNullOrEmpty(assemblyName))
                {
                    query = query.Where(t => t.Assembly == assemblyName);
                }
                else
                {
                    query = query.Where(t => string.IsNullOrEmpty(t.Assembly));
                }

                var theme = await query
                    .OrderByDescending(t => t.CreatedAt)
                    .FirstOrDefaultAsync();

                if (theme == null)
                {
                    return new DbResponse<MonthlyTheme>
                    {
                        Success = false,
                        Message = "No active theme found for the current month."
                    };
                }

                return new DbResponse<MonthlyTheme>
                {
                    Success = true,
                    Data = theme
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<MonthlyTheme>
                {
                    Success = false,
                    Message = $"Error fetching theme for current month: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<MonthlyTheme>> UpdateMonthlyThemeAsync(int id, MonthlyThemeDto dto, string? assemblyName = null)
        {
            try
            {
                MonthlyTheme existing = null;

                if (id > 0)
                {
                    existing = await _context.MonthlyThemes.FindAsync(id);
                }
                else
                {
                    var query = _context.MonthlyThemes.Where(t => t.Year == dto.Year && t.Month == dto.Month);
                    if (!string.IsNullOrEmpty(assemblyName))
                        query = query.Where(t => t.Assembly == assemblyName);
                    else
                        query = query.Where(t => string.IsNullOrEmpty(t.Assembly));

                    existing = await query.FirstOrDefaultAsync();
                }

                if (existing == null || existing.Year != dto.Year || existing.Month != dto.Month)
                {
                    var newTheme = new MonthlyTheme
                    {
                        Theme = dto.Theme,
                        Description = dto.Description,
                        Month = dto.Month,
                        Year = dto.Year,
                        Assembly = assemblyName,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.MonthlyThemes.Add(newTheme);
                    await _context.SaveChangesAsync();

                    Loggers.DoLogs($"Created new monthly theme: {newTheme.Theme} for {newTheme.Month}/{newTheme.Year}");

                    return new DbResponse<MonthlyTheme>
                    {
                        Success = true,
                        Message = "Monthly theme created successfully",
                        Data = newTheme
                    };
                }

                existing.Theme = dto.Theme;
                existing.Description = dto.Description;
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    existing.Assembly = assemblyName;
                }
                existing.IsActive = true;
                existing.UpdatedAt = DateTime.Now;

                Loggers.DoLogs($"Updated monthly theme: {existing.Theme} for {existing.Month}/{existing.Year}");

                await _context.SaveChangesAsync();

                return new DbResponse<MonthlyTheme>
                {
                    Success = true,
                    Message = "Monthly theme updated successfully",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"UpdateMonthlyThemeAsync Error: {ex}");

                return new DbResponse<MonthlyTheme>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }


        public async Task<DbResponse<List<Event>>> GetUpcomingEventsAsync()
        {
            try
            {
                var today = DateTime.Today;
                var nextWeek = today.AddDays(7);
                var upcomingEvents = await _context.Events
                    .Where(e => e.EventDate >= today && e.EventDate <= nextWeek && e.IsActive)
                    .OrderBy(e => e.EventDate)
                    .ToListAsync();
                return new DbResponse<List<Event>>
                {
                    Success = true,
                    Data = upcomingEvents
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Event>>
                {
                    Success = false,
                    Message = $"Error fetching upcoming events: {ex.Message}"
                };
            }
        }

        // Add these methods to your EventsRepository class

        public async Task<DbResponse<List<Event>>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var events = await _context.Events
                    .Where(e => e.EventDate >= startDate && e.EventDate <= endDate)
                    .OrderBy(e => e.EventDate)
                    .ToListAsync();

                return new DbResponse<List<Event>>
                {
                    Success = true,
                    Data = events,
                    Message = $"Found {events.Count} events between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching events by date range: {ex.ToString()}");
                return new DbResponse<List<Event>>
                {
                    Success = false,
                    Message = $"Error fetching events by date range: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<Event>>> GetUpcomingEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var now = DateTime.Now;
                var events = await _context.Events
                    .Where(e => e.EventDate >= startDate && e.EventDate <= endDate && e.EventDate >= now)
                    .OrderBy(e => e.EventDate)
                    .ToListAsync();

                return new DbResponse<List<Event>>
                {
                    Success = true,
                    Data = events,
                    Message = $"Found {events.Count} upcoming events between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching upcoming events by date range: {ex.ToString()}");
                return new DbResponse<List<Event>>
                {
                    Success = false,
                    Message = $"Error fetching upcoming events by date range: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<int>> GetEventsCountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var count = await _context.Events
                    .CountAsync(e => e.EventDate >= startDate && e.EventDate <= endDate);

                return new DbResponse<int>
                {
                    Success = true,
                    Data = count,
                    Message = $"Found {count} events between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error counting events by date range: {ex.ToString()}");
                return new DbResponse<int>
                {
                    Success = false,
                    Message = $"Error counting events by date range: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<int>> GetUpcomingEventsCountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var now = DateTime.Now;
                var count = await _context.Events
                    .CountAsync(e => e.EventDate >= startDate && e.EventDate <= endDate && e.EventDate >= now);

                return new DbResponse<int>
                {
                    Success = true,
                    Data = count,
                    Message = $"Found {count} upcoming events between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error counting upcoming events by date range: {ex.ToString()}");
                return new DbResponse<int>
                {
                    Success = false,
                    Message = $"Error counting upcoming events by date range: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<Dictionary<DateTime, int>>> GetEventsGroupedByDateAsync(DateTime startDate, DateTime endDate, string groupBy = "day")
        {
            try
            {
                var query = _context.Events
                    .Where(e => e.EventDate >= startDate && e.EventDate <= endDate);

                Dictionary<DateTime, int> groupedData = new Dictionary<DateTime, int>();

                if (groupBy.ToLower() == "day")
                {
                    groupedData = await query
                        .GroupBy(e => e.EventDate.Date)
                        .Select(g => new { Date = g.Key, Count = g.Count() })
                        .ToDictionaryAsync(g => g.Date, g => g.Count);
                }
                else if (groupBy.ToLower() == "week")
                {
                    groupedData = await query
                        .GroupBy(e => new {
                            Year = e.EventDate.Year,
                            Week = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                e.EventDate,
                                System.Globalization.CalendarWeekRule.FirstDay,
                                DayOfWeek.Sunday)
                        })
                        .Select(g => new {
                            Date = new DateTime(g.Key.Year, 1, 1).AddDays((g.Key.Week - 1) * 7),
                            Count = g.Count()
                        })
                        .ToDictionaryAsync(g => g.Date, g => g.Count);
                }
                else if (groupBy.ToLower() == "month")
                {
                    groupedData = await query
                        .GroupBy(e => new { e.EventDate.Year, e.EventDate.Month })
                        .Select(g => new { Date = new DateTime(g.Key.Year, g.Key.Month, 1), Count = g.Count() })
                        .ToDictionaryAsync(g => g.Date, g => g.Count);
                }
                else if (groupBy.ToLower() == "year")
                {
                    groupedData = await query
                        .GroupBy(e => e.EventDate.Year)
                        .Select(g => new { Date = new DateTime(g.Key, 1, 1), Count = g.Count() })
                        .ToDictionaryAsync(g => g.Date, g => g.Count);
                }

                return new DbResponse<Dictionary<DateTime, int>>
                {
                    Success = true,
                    Data = groupedData,
                    Message = $"Found {groupedData.Count} date groups"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching events grouped by date: {ex.ToString()}");
                return new DbResponse<Dictionary<DateTime, int>>
                {
                    Success = false,
                    Message = $"Error fetching events grouped by date: {ex.Message}"
                };
            }
        }

     
        public async Task<DbResponse<List<Event>>> GetEventsForWeekAsync(DateTime date)
        {
            try
            {
                // Get Sunday of the week (assuming Sunday is first day of week)
                var startOfWeek = date.AddDays(-(int)date.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(7).AddSeconds(-1);

                var events = await _context.Events
                    .Where(e => e.EventDate >= startOfWeek && e.EventDate <= endOfWeek)
                    .OrderBy(e => e.EventDate)
                    .ToListAsync();

                return new DbResponse<List<Event>>
                {
                    Success = true,
                    Data = events,
                    Message = $"Found {events.Count} events for week starting {startOfWeek:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching events for week: {ex.ToString()}");
                return new DbResponse<List<Event>>
                {
                    Success = false,
                    Message = $"Error fetching events for week: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<Event>>> GetEventsForMonthAsync(int year, int month)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddSeconds(-1);

                var events = await _context.Events
                    .Where(e => e.EventDate >= startDate && e.EventDate <= endDate)
                    .OrderBy(e => e.EventDate)
                    .ToListAsync();

                return new DbResponse<List<Event>>
                {
                    Success = true,
                    Data = events,
                    Message = $"Found {events.Count} events in {startDate:MMMM yyyy}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching events for month: {ex.ToString()}");
                return new DbResponse<List<Event>>
                {
                    Success = false,
                    Message = $"Error fetching events for month: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<Event>>> GetEventsForYearAsync(int year)
        {
            try
            {
                var startDate = new DateTime(year, 1, 1);
                var endDate = new DateTime(year, 12, 31, 23, 59, 59);

                var events = await _context.Events
                    .Where(e => e.EventDate >= startDate && e.EventDate <= endDate)
                    .OrderBy(e => e.EventDate)
                    .ToListAsync();

                return new DbResponse<List<Event>>
                {
                    Success = true,
                    Data = events,
                    Message = $"Found {events.Count} events in {year}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching events for year: {ex.ToString()}");
                return new DbResponse<List<Event>>
                {
                    Success = false,
                    Message = $"Error fetching events for year: {ex.Message}"
                };
            }
        }
        //get even registrations by event id
        public async Task<DbResponse<List<EventRegistration>>> GetEventRegistrationsByEventIdAsync(int eventId)
        {
            try
            {
                var registrations = await _context.EventRegistrations
                    .Where(r => r.EventId == eventId)
                    .OrderByDescending(r => r.RegistrationDate)
                    .ToListAsync();

                var memberIds = registrations.Where(r => r.MemberId != 0).Select(r => r.MemberId).Distinct().ToList();
                if (memberIds.Any())
                {
                    var members = await _context.Members.Where(m => memberIds.Contains(m.Id)).ToDictionaryAsync(m => m.Id);
                    foreach (var r in registrations.Where(r => r.MemberId != 0))
                    {
                        if (members.TryGetValue(r.MemberId, out var member))
                        {
                            r.Member = member;
                        }
                    }
                }
                return new DbResponse<List<EventRegistration>>
                {
                    Success = true,
                    Data = registrations,
                    Message = $"Found {registrations.Count} registrations for event ID {eventId}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching registrations for event ID {eventId}: {ex.ToString()}");
                return new DbResponse<List<EventRegistration>>
                {
                    Success = false,
                    Message = $"Error fetching registrations for event ID {eventId}: {ex.Message}"
                };
            }
        }


        }
}

