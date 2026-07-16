using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class RcpsRepository
    {
        private readonly AppDbContext _context;

        public RcpsRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // ✅ CREATE
        // =========================================================
        public async Task<DbResponse<Rcps>> CreateRcpsAsync(RcpsDto dto)
        {
            try
            {
                var entity = new Rcps
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    TargetAmount = dto.TargetAmount,
                    AmountRaised = dto.AmountRaised,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Status = dto.Status,
                    CountyCode = dto.CountyCode,
                    IsActive = false,
                    CreatedAt = DateTime.Now
                };

                _context.Rcps.Add(entity);
                await _context.SaveChangesAsync();

                return new DbResponse<Rcps>
                {
                    Success = true,
                    Message = "Rcps created successfully",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"CreateRcpsAsync Error: {ex}");

                return new DbResponse<Rcps>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ GET ALL
        // =========================================================
        public async Task<DbResponse<List<Rcps>>> GetAllRcpsAsync()
        {
            try
            {
                var data = await _context.Rcps
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<Rcps>>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetAllRcpsAsync Error: {ex}");

                return new DbResponse<List<Rcps>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ GET BY ID
        // =========================================================
        public async Task<DbResponse<Rcps>> GetRcpsByIdAsync(int id)
        {
            try
            {
                var data = await _context.Rcps
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (data == null)
                {
                    return new DbResponse<Rcps>
                    {
                        Success = false,
                        Message = "Rcps not found"
                    };
                }

                return new DbResponse<Rcps>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetRcpsByIdAsync Error: {ex}");

                return new DbResponse<Rcps>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ UPDATE
        // =========================================================
        public async Task<DbResponse<Rcps>> UpdateRcpsAsync(Rcps dto)
        {
            try
            {
                var existing = await _context.Rcps.FindAsync(dto.Id);
                if (existing == null)
                {
                    return new DbResponse<Rcps>
                    {
                        Success = false,
                        Message = "Rcps not found"
                    };
                }

                existing.Name = dto.Name;
                existing.Description = dto.Description;
                existing.TargetAmount = dto.TargetAmount;
                existing.AmountRaised = dto.AmountRaised;
                existing.StartDate = dto.StartDate;
                existing.EndDate = dto.EndDate;
                existing.Status = dto.Status;
                existing.CountyCode = dto.CountyCode;
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<Rcps>
                {
                    Success = true,
                    Message = "Rcps updated successfully",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"UpdateRcpsAsync Error: {ex}");

                return new DbResponse<Rcps>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ DELETE (SOFT DELETE)
        // =========================================================
        public async Task<DbResponse<bool>> DeleteRcpsAsync(int id)
        {
            try
            {
                var data = await _context.Rcps.FindAsync(id);

                if (data == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Rcps not found"
                    };
                }

                data.IsActive = false;
                data.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Rcps deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"DeleteRcpsAsync Error: {ex}");

                return new DbResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ CREATE
        // =========================================================
        public async Task<DbResponse<RcpsPledges>> CreateRcpsPledgeAsync(RcpsPledgesDto dto)
        {
            try
            {
                var entity = new RcpsPledges
                {
                    MemberId = dto.MemberId,
                    RcpsId = dto.RcpsId,
                    PledgedAmount = dto.PledgedAmount,
                    AmountPaid = dto.AmountPaid,
                    PledgeDate = dto.PledgeDate,
                    TargetCompletionDate = dto.TargetCompletionDate,
                    Notes = dto.Notes,
                    Status = dto.Status,
                    PaymentRecieved = dto.PaymentRecieved,
                    Balance = dto.PledgedAmount - dto.AmountPaid,
                    CreatedAt = DateTime.Now
                };

                _context.RcpsPledges.Add(entity);
                await _context.SaveChangesAsync();

                return new DbResponse<RcpsPledges>
                {
                    Success = true,
                    Message = "Pledge created successfully",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"CreateRcpsPledgeAsync Error: {ex}");

                return new DbResponse<RcpsPledges>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ GET ALL
        // =========================================================
        public async Task<DbResponse<List<RcpsPledges>>> GetAllRcpsPledgesAsync()
        {
            try
            {
                var data = await _context.RcpsPledges
                    .Include(x => x.Member)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<RcpsPledges>>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetAllRcpsPledgesAsync Error: {ex}");

                return new DbResponse<List<RcpsPledges>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ GET BY ID
        // =========================================================
        public async Task<DbResponse<RcpsPledges>> GetRcpsPledgeByIdAsync(int id)
        {
            try
            {
                var data = await _context.RcpsPledges
                    .Include(x => x.Member)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (data == null)
                {
                    return new DbResponse<RcpsPledges>
                    {
                        Success = false,
                        Message = "Pledge not found"
                    };
                }

                return new DbResponse<RcpsPledges>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetRcpsPledgeByIdAsync Error: {ex}");

                return new DbResponse<RcpsPledges>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ UPDATE
        // =========================================================
        public async Task<DbResponse<RcpsPledges>> UpdateRcpsPledgeAsync(int id, RcpsPledgesDto dto)
        {
            try
            {
                var existing = await _context.RcpsPledges.FindAsync(id);

                if (existing == null)
                {
                    return new DbResponse<RcpsPledges>
                    {
                        Success = false,
                        Message = "Pledge not found"
                    };
                }

                existing.MemberId = dto.MemberId;
                existing.RcpsId = dto.RcpsId;
                existing.PledgedAmount = dto.PledgedAmount;
                existing.AmountPaid = dto.AmountPaid;
                existing.PledgeDate = dto.PledgeDate;
                existing.TargetCompletionDate = dto.TargetCompletionDate;
                existing.Notes = dto.Notes;
                existing.Status = dto.Status;
                existing.PaymentRecieved = dto.PaymentRecieved;
                existing.Balance = dto.PledgedAmount - dto.AmountPaid;
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<RcpsPledges>
                {
                    Success = true,
                    Message = "Pledge updated successfully",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"UpdateRcpsPledgeAsync Error: {ex}");

                return new DbResponse<RcpsPledges>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // ✅ DELETE (SOFT)
        // =========================================================
        public async Task<DbResponse<bool>> DeleteRcpsPledgeAsync(int id)
        {
            try
            {
                var data = await _context.RcpsPledges.FindAsync(id);

                if (data == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Pledge not found"
                    };
                }

                _context.RcpsPledges.Remove(data);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Pledge deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"DeleteRcpsPledgeAsync Error: {ex}");

                return new DbResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<RcpsPledges>>> GetPledgesByRcpsIdAsync(int id)
        {
            try
            {
                var data = await _context.RcpsPledges
                    .Include(x => x.Member)
                    .Where(x => x.RcpsId == id)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<RcpsPledges>>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"GetPledgesByRcpsIdAsync Error: {ex}");

                return new DbResponse<List<RcpsPledges>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // =========================================================
        // COUNTY COORDINATORS CRUD
        // =========================================================

        public async Task<DbResponse<RcpsCountyCoordinator>> CreateRcpsCountyCoordinatorAsync(RcpsCountyCoordinatorDto dto)
        {
            try
            {
                var coordinator = new RcpsCountyCoordinator
                {
                    MemberId = dto.MemberId,
                    RcpsId = dto.RcpsId,
                    Bio = dto.Bio,
                    IsActive = dto.IsActive
                };

                _context.RcpsCountyCoordinators.Add(coordinator);
                await _context.SaveChangesAsync();

                return new DbResponse<RcpsCountyCoordinator>
                {
                    Success = true,
                    Data = coordinator,
                    Message = "County Coordinator created successfully"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error creating county coordinator: {ex}");
                return new DbResponse<RcpsCountyCoordinator>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<RcpsCountyCoordinator>> GetRcpsCountyCoordinatorByIdAsync(int id)
        {
            try
            {
                var coordinator = await _context.RcpsCountyCoordinators
                    .Include(c => c.Member)
                    .Include(c => c.Rcps)
                    .FirstOrDefaultAsync(c => c.RcpsCountyCoordinatorId == id);

                return new DbResponse<RcpsCountyCoordinator>
                {
                    Success = coordinator != null,
                    Data = coordinator,
                    Message = coordinator != null ? "Success" : "Not found"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error getting county coordinator: {ex}");
                return new DbResponse<RcpsCountyCoordinator>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<RcpsCountyCoordinator>>> GetAllRcpsCountyCoordinatorsAsync()
        {
            try
            {
                var coordinators = await _context.RcpsCountyCoordinators
                    .Include(x => x.Member)
                    .Include(x => x.Rcps)
                    .ToListAsync();
                return new DbResponse<List<RcpsCountyCoordinator>>
                {
                    Success = true,
                    Data = coordinators
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error getting all county coordinators: {ex}");
                return new DbResponse<List<RcpsCountyCoordinator>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<List<RcpsCountyCoordinator>>> GetRcpsCountyCoordinatorsByRcpsAsync(int rcpsId)
        {
            try
            {
                var coordinators = await _context.RcpsCountyCoordinators
                    .Include(c => c.Member)
                    .Include(c => c.Rcps)
                    .Where(c => c.RcpsId == rcpsId)
                    .ToListAsync();

                return new DbResponse<List<RcpsCountyCoordinator>>
                {
                    Success = true,
                    Data = coordinators
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error getting county coordinators by Rcps: {ex}");
                return new DbResponse<List<RcpsCountyCoordinator>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<RcpsCountyCoordinator>> UpdateRcpsCountyCoordinatorAsync(RcpsCountyCoordinatorDto dto)
        {
            try
            {
                var coordinator = await _context.RcpsCountyCoordinators.FindAsync(dto.Id);
                if (coordinator == null)
                {
                    return new DbResponse<RcpsCountyCoordinator>
                    {
                        Success = false,
                        Message = "County Coordinator not found"
                    };
                }

                coordinator.MemberId = dto.MemberId;
                coordinator.RcpsId = dto.RcpsId;
                coordinator.Bio = dto.Bio;
                coordinator.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                return new DbResponse<RcpsCountyCoordinator>
                {
                    Success = true,
                    Data = coordinator,
                    Message = "County Coordinator updated successfully"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error updating county coordinator: {ex}");
                return new DbResponse<RcpsCountyCoordinator>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<bool>> DeleteRcpsCountyCoordinatorAsync(int id)
        {
            try
            {
                var coordinator = await _context.RcpsCountyCoordinators.FindAsync(id);
                if (coordinator == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "County Coordinator not found"
                    };
                }

                coordinator.IsActive = false;
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "County Coordinator soft deleted successfully"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error deleting county coordinator: {ex}");
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<DbResponse<bool>> ToggleCountyCoordinatorStatusAsync(int id, bool isActive)
        {
            try
            {
                var coordinator = await _context.RcpsCountyCoordinators.FindAsync(id);
                if (coordinator == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "County Coordinator not found"
                    };
                }

                coordinator.IsActive = isActive;
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = $"County Coordinator status updated to {(isActive ? "Active" : "Inactive")}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error toggling county coordinator status: {ex}");
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}