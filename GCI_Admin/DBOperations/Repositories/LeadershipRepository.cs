using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class LeadershipRepository
    {
        private readonly AppDbContext _context;

        public LeadershipRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // ✅ CREATE DEACON
        // =========================================================
        public async Task<DbResponse<Deacon>> CreateDeaconAsync(DeaconDto dto)
        {
            try
            {
                var memberExists = await _context.Members
                    .AnyAsync(x => x.Id == dto.MemberId);

                if (!memberExists)
                    return new DbResponse<Deacon>
                    {
                        Success = false,
                        Message = "Member not found"
                    };

                var entity = new Deacon
                {
                    MemberId = dto.MemberId,
                    Ministry = dto.MinistryId,
                    Bio = dto.Bio,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    OnDuty = dto.OnDuty,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.Deacons.Add(entity);
                await _context.SaveChangesAsync();

                return new DbResponse<Deacon>
                {
                    Success = true,
                    Message = "Deacon created successfully",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"CreateDeaconAsync Error: {ex}");
                return new DbResponse<Deacon>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ GET ALL DEACONS
        // =========================================================
        public async Task<DbResponse<List<Deacon>>> GetAllDeaconsAsync()
        {
            try
            {
                var data = await _context.Deacons
                    .Where(x => x.IsActive)
                    .Include(x => x.Member)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<Deacon>>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetAllDeaconsAsync Error: {ex}");
                return new DbResponse<List<Deacon>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ GET BY ID
        // =========================================================
        public async Task<DbResponse<Deacon>> GetDeaconByIdAsync(int id)
        {
            try
            {
                var data = await _context.Deacons
                    .Include(x => x.Member)
                    .FirstOrDefaultAsync(x => x.DeaconId == id);

                if (data == null)
                    return new DbResponse<Deacon>
                    {
                        Success = false,
                        Message = "Deacon not found"
                    };

                return new DbResponse<Deacon>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Deacon>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ UPDATE
        // =========================================================
        public async Task<DbResponse<Deacon>> UpdateDeaconAsync(int id, DeaconDto dto)
        {
            try
            {
                var existing = await _context.Deacons.FindAsync(id);

                if (existing == null)
                    return new DbResponse<Deacon>
                    {
                        Success = false,
                        Message = "Deacon not found"
                    };

                existing.MemberId = dto.MemberId;
                existing.Ministry = dto.MinistryId;
                existing.Bio = dto.Bio;
                existing.StartDate = dto.StartDate;
                existing.EndDate = dto.EndDate;
                existing.OnDuty = dto.OnDuty;

                await _context.SaveChangesAsync();

                return new DbResponse<Deacon>
                {
                    Success = true,
                    Message = "Deacon updated successfully",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"UpdateDeaconAsync Error: {ex}");
                return new DbResponse<Deacon>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ DELETE (SOFT)
        // =========================================================
        public async Task<DbResponse<bool>> DeleteDeaconAsync(int id)
        {
            try
            {
                var data = await _context.Deacons.FindAsync(id);

                if (data == null)
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Deacon not found"
                    };

                data.IsActive = false;

                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Deacon deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"DeleteDeaconAsync Error: {ex}");
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ TOGGLE DUTY STATUS
        // =========================================================
        public async Task<DbResponse<bool>> ToggleDutyStatusAsync(int id, bool onDuty)
        {
            try
            {
                var data = await _context.Deacons.FindAsync(id);

                if (data == null)
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Deacon not found"
                    };

                data.OnDuty = onDuty;

                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = onDuty ? "Marked as on duty" : "Marked as off duty",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}