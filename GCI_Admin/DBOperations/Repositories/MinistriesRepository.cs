using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class MinistriesRepository
    {
        private readonly AppDbContext _context;

        public MinistriesRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ CREATE MINISTRY
        public async Task<DbResponse<Ministry>> CreateMinistryAsync(MinistryDto dto)
        {
            try
            {
                var newMinistry = new Ministry
                {
                    MinistryName = dto.MinistryName,
                    Description = dto.Description,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Ministries.Add(newMinistry);
                await _context.SaveChangesAsync();

                return new DbResponse<Ministry>
                {
                    Success = true,
                    Message = "Ministry created successfully",
                    Data = newMinistry
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in CreateMinistryAsync: {ex}");
                return new DbResponse<Ministry>
                {
                    Success = false,
                    Message = $"Error creating ministry: {ex.Message}"
                };
            }
        }

        // ✅ GET ALL MINISTRIES
        public async Task<DbResponse<List<Ministry>>> GetAllMinistriesAsync()
        {
            try
            {
                var ministries = await _context.Ministries
                    .OrderBy(m => m.MinistryId)
                    .ToListAsync();

                return new DbResponse<List<Ministry>>
                {
                    Success = true,
                    Data = ministries
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Ministry>>
                {
                    Success = false,
                    Message = $"Error fetching ministries: {ex.Message}"
                };
            }
        }

        // ✅ GET MINISTRY BY ID
        public async Task<DbResponse<Ministry>> GetMinistryByIdAsync(int ministryId)
        {
            try
            {
                var ministry = await _context.Ministries
                    .FirstOrDefaultAsync(m => m.MinistryId == ministryId);

                if (ministry == null)
                    return new DbResponse<Ministry>
                    {
                        Success = false,
                        Message = "Ministry not found"
                    };

                return new DbResponse<Ministry>
                {
                    Success = true,
                    Data = ministry
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Ministry>
                {
                    Success = false,
                    Message = $"Error fetching ministry: {ex.Message}"
                };
            }
        }

        // ✅ UPDATE MINISTRY
        public async Task<DbResponse<Ministry>> UpdateMinistryAsync(int ministryId, MinistryDto dto)
        {
            try
            {
                var ministry = await _context.Ministries.FindAsync(ministryId);

                if (ministry == null)
                    return new DbResponse<Ministry>
                    {
                        Success = false,
                        Message = "Ministry not found"
                    };

                ministry.MinistryName = dto.MinistryName;
                ministry.Description = dto.Description;
                ministry.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<Ministry>
                {
                    Success = true,
                    Message = "Ministry updated successfully",
                    Data = ministry
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Ministry>
                {
                    Success = false,
                    Message = $"Error updating ministry: {ex.Message}"
                };
            }
        }

        // ✅ DELETE MINISTRY
        public async Task<DbResponse<bool>> DeleteMinistryAsync(int ministryId)
        {
            try
            {
                var ministry = await _context.Ministries.FindAsync(ministryId);

                if (ministry == null)
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Ministry not found"
                    };

                _context.Ministries.Remove(ministry);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Ministry deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting ministry: {ex.Message}"
                };
            }
        }

        // ✅ TOGGLE ACTIVE STATUS
        public async Task<DbResponse<bool>> ToggleMinistryStatusAsync(int ministryId, bool isActive)
        {
            try
            {
                var ministry = await _context.Ministries.FindAsync(ministryId);

                if (ministry == null)
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Ministry not found"
                    };

                ministry.IsActive = isActive;
                ministry.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = isActive ? "Ministry activated successfully" : "Ministry deactivated successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating ministry status: {ex.Message}"
                };
            }
        }

        // ✅ GET ALL MINISTRY LEADERS
        public async Task<DbResponse<List<MinistryLeader>>> GetMinistryLeadersAsync()
        {
            try
            {
                var leaders = await _context.MinistryLeaders
                    .AsNoTracking()
                    .Where(l => l.IsActive)
                    .Include(l => l.Member)
                    .Include(l => l.Ministry)
                    .OrderByDescending(l => l.StartDate)
                    .ToListAsync();

                return new DbResponse<List<MinistryLeader>>
                {
                    Success = true,
                    Data = leaders
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching ministry leaders: {ex}");
                return new DbResponse<List<MinistryLeader>>
                {
                    Success = false,
                    Message = $"Error fetching ministry leaders: {ex.Message}"
                };
            }
        }
    }
}