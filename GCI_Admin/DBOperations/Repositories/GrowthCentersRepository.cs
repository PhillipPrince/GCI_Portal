using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class GrowthCentersRepository
    {
        private readonly AppDbContext _context;

        public GrowthCentersRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ CREATE GROWTH CENTER
        public async Task<DbResponse<GrowthCenter>> CreateGrowthCenterAsync(GrowthCenterDto dto)
        {
            try
            {
                var newCenter = new GrowthCenter
                {
                    CenterName = dto.CenterName,
                    Location = dto.Location,
                    Description = dto.Description,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.GrowthCenters.Add(newCenter);
                await _context.SaveChangesAsync();

                return new DbResponse<GrowthCenter>
                {
                    Success = true,
                    Message = "Growth center created successfully",
                    Data = newCenter
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in CreateGrowthCenterAsync: {ex}");
                return new DbResponse<GrowthCenter>
                {
                    Success = false,
                    Message = $"Error creating growth center: {ex.Message}"
                };
            }
        }

        // ✅ GET ALL GROWTH CENTERS
        public async Task<DbResponse<List<GrowthCenter>>> GetAllGrowthCentersAsync()
        {
            try
            {
                var centers = await _context.GrowthCenters
                    .OrderBy(c => c.GrowthCenterId)
                    .ToListAsync();

                return new DbResponse<List<GrowthCenter>>
                {
                    Success = true,
                    Data = centers
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<GrowthCenter>>
                {
                    Success = false,
                    Message = $"Error fetching growth centers: {ex.Message}"
                };
            }
        }

        // ✅ GET GROWTH CENTER BY ID
        public async Task<DbResponse<GrowthCenter>> GetGrowthCenterByIdAsync(int centerId)
        {
            try
            {
                var center = await _context.GrowthCenters
                    .FirstOrDefaultAsync(c => c.GrowthCenterId == centerId);

                if (center == null)
                    return new DbResponse<GrowthCenter>
                    {
                        Success = false,
                        Message = "Growth center not found"
                    };

                return new DbResponse<GrowthCenter>
                {
                    Success = true,
                    Data = center
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<GrowthCenter>
                {
                    Success = false,
                    Message = $"Error fetching growth center: {ex.Message}"
                };
            }
        }

        // ✅ UPDATE GROWTH CENTER
        public async Task<DbResponse<GrowthCenter>> UpdateGrowthCenterAsync(int centerId, GrowthCenterDto dto)
        {
            try
            {
                var center = await _context.GrowthCenters.FindAsync(centerId);
                if (center == null)
                    return new DbResponse<GrowthCenter>
                    {
                        Success = false,
                        Message = "Growth center not found"
                    };

                center.CenterName = dto.CenterName;
                center.Location = dto.Location;
                center.Description = dto.Description;
                center.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<GrowthCenter>
                {
                    Success = true,
                    Message = "Growth center updated successfully",
                    Data = center
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<GrowthCenter>
                {
                    Success = false,
                    Message = $"Error updating growth center: {ex.Message}"
                };
            }
        }

        // ✅ DELETE GROWTH CENTER
        public async Task<DbResponse<bool>> DeleteGrowthCenterAsync(int centerId)
        {
            try
            {
                var center = await _context.GrowthCenters.FindAsync(centerId);
                if (center == null)
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Growth center not found"
                    };

                _context.GrowthCenters.Remove(center);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Growth center deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting growth center: {ex.Message}"
                };
            }
        }

        // ✅ TOGGLE ACTIVE STATUS
        public async Task<DbResponse<bool>> ToggleGrowthCenterStatusAsync(int centerId, bool isActive)
        {
            try
            {
                var center = await _context.GrowthCenters.FindAsync(centerId);
                if (center == null)
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Growth center not found"
                    };

                center.IsActive = isActive;
                center.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = isActive ? "Growth center activated successfully" : "Growth center deactivated successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating growth center status: {ex.Message}"
                };
            }
        }

        // ✅ GET ALL ACTIVE GROWTH CENTER LEADERS
        public async Task<DbResponse<List<GrowthCenterLeader>>> GetGrowthCenterLeadersAsync()
        {
            try
            {
                var leaders = await _context.GrowthCenterLeaders
                    .AsNoTracking()
                    .Where(l => l.IsActive)
                    .Include(l => l.Member)
                    .Include(l => l.GrowthCenter)
                    .OrderByDescending(l => l.StartDate)
                    .ToListAsync();

                return new DbResponse<List<GrowthCenterLeader>>
                {
                    Success = true,
                    Data = leaders
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching growth center leaders: {ex}");
                return new DbResponse<List<GrowthCenterLeader>>
                {
                    Success = false,
                    Message = $"Error fetching growth center leaders: {ex.Message}"
                };
            }
        }
    }
}