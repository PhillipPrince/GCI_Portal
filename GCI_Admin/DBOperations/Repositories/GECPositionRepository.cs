using GCI_Admin.Models;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class GECPositionRepository
    {
        private readonly AppDbContext _context;

        public GECPositionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse<List<GECPosition>>> GetAllPositionsAsync()
        {
            try
            {
                var positions = await _context.GECPositions
                    .OrderBy(p => p.Id)
                    .ToListAsync();

                return new DbResponse<List<GECPosition>>
                {
                    Success = true,
                    Data = positions
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<GECPosition>>
                {
                    Success = false,
                    Message = $"Error fetching GEC positions: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<GECPosition>> GetPositionByIdAsync(int id)
        {
            try
            {
                var position = await _context.GECPositions
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (position == null)
                {
                    return new DbResponse<GECPosition>
                    {
                        Success = false,
                        Message = "GEC position not found"
                    };
                }

                return new DbResponse<GECPosition>
                {
                    Success = true,
                    Data = position
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<GECPosition>
                {
                    Success = false,
                    Message = $"Error fetching GEC position: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<GECPosition>> CreatePositionAsync(GECPosition position)
        {
            try
            {
                position.CreatedAt = DateTime.Now;
                _context.GECPositions.Add(position);
                await _context.SaveChangesAsync();

                return new DbResponse<GECPosition>
                {
                    Success = true,
                    Message = "GEC position created successfully",
                    Data = position
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in CreatePositionAsync: {ex}");
                return new DbResponse<GECPosition>
                {
                    Success = false,
                    Message = $"Error creating GEC position: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<GECPosition>> UpdatePositionAsync(GECPosition position)
        {
            try
            {
                var existing = await _context.GECPositions.FindAsync(position.Id);

                if (existing == null)
                {
                    return new DbResponse<GECPosition>
                    {
                        Success = false,
                        Message = "GEC position not found"
                    };
                }

                existing.Title = position.Title;
                existing.Description = position.Description;
                existing.IsActive = position.IsActive;
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<GECPosition>
                {
                    Success = true,
                    Message = "GEC position updated successfully",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in UpdatePositionAsync: {ex}");
                return new DbResponse<GECPosition>
                {
                    Success = false,
                    Message = $"Error updating GEC position: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<bool>> DeletePositionAsync(int id)
        {
            try
            {
                var position = await _context.GECPositions.FindAsync(id);

                if (position == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "GEC position not found"
                    };
                }

                // Instead of hard delete, we check if it is in use.
                bool inUse = await _context.GECMembers.AnyAsync(m => m.GECPositionId == id);
                if (inUse)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Cannot delete position because it is assigned to one or more GEC members."
                    };
                }

                _context.GECPositions.Remove(position);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "GEC position deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting GEC position: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<bool>> TogglePositionStatusAsync(int id, bool isActive)
        {
            try
            {
                var position = await _context.GECPositions.FindAsync(id);

                if (position == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "GEC position not found"
                    };
                }

                position.IsActive = isActive;
                position.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = isActive ? "GEC position activated successfully" : "GEC position deactivated successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating status: {ex.Message}"
                };
            }
        }
    }
}
