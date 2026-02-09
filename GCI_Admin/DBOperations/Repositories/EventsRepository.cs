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
                    CreatedAt = DateTime.Now
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
                Loggers.DoLogs($"Error in CreateEventAsync: {ex.ToString()}");
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
                    .OrderByDescending(e => e.EventDate)
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
                existingEvent.UpdatedAt = DateTime.Now;

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
    }
}
