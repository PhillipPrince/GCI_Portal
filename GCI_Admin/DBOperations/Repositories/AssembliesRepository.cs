using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class AssembliesRepository
    {
        private readonly AppDbContext _context;

        public AssembliesRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ CREATE ASSEMBLY
        public async Task<DbResponse<Assembly>> CreateAssemblyAsync(AssemblyDto dto)
        {
            try
            {
                var newAssembly = new Assembly
                {
                    Name = dto.Name,
                    Location = dto.Location,
                    ContactPhone = dto.ContactPhone,
                    ContactEmail = dto.ContactEmail,
                    CreatedAt = DateTime.Now
                };

                _context.Assemblies.Add(newAssembly);
                await _context.SaveChangesAsync();

                return new DbResponse<Assembly>
                {
                    Success = true,
                    Message = "Assembly created successfully",
                    Data = newAssembly
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in CreateAssemblyAsync: {ex}");
                return new DbResponse<Assembly>
                {
                    Success = false,
                    Message = $"Error creating assembly: {ex.Message}"
                };
            }
        }

        // ✅ GET ALL ASSEMBLIES
        public async Task<DbResponse<List<Assembly>>> GetAllAssembliesAsync()
        {
            try
            {
                var assemblies = await _context.Assemblies
                    .OrderBy(a => a.Id)
                    .ToListAsync();

                return new DbResponse<List<Assembly>>
                {
                    Success = true,
                    Data = assemblies
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<Assembly>>
                {
                    Success = false,
                    Message = $"Error fetching assemblies: {ex.Message}"
                };
            }
        }

        // ✅ GET ASSEMBLY BY ID
        public async Task<DbResponse<Assembly>> GetAssemblyByIdAsync(int assemblyId)
        {
            try
            {
                var assembly = await _context.Assemblies
                    .FirstOrDefaultAsync(a => a.Id == assemblyId);

                if (assembly == null)
                {
                    return new DbResponse<Assembly>
                    {
                        Success = false,
                        Message = "Assembly not found"
                    };
                }

                return new DbResponse<Assembly>
                {
                    Success = true,
                    Data = assembly
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Assembly>
                {
                    Success = false,
                    Message = $"Error fetching assembly: {ex.Message}"
                };
            }
        }

        // ✅ UPDATE ASSEMBLY
        public async Task<DbResponse<Assembly>> UpdateAssemblyAsync(int assemblyId, AssemblyDto dto)
        {
            try
            {
                var existingAssembly = await _context.Assemblies.FindAsync(assemblyId);

                if (existingAssembly == null)
                {
                    return new DbResponse<Assembly>
                    {
                        Success = false,
                        Message = "Assembly not found"
                    };
                }

                existingAssembly.Name = dto.Name;
                existingAssembly.Location = dto.Location;
                existingAssembly.ContactPhone = dto.ContactPhone;
                existingAssembly.ContactEmail = dto.ContactEmail;

                await _context.SaveChangesAsync();

                return new DbResponse<Assembly>
                {
                    Success = true,
                    Message = "Assembly updated successfully",
                    Data = existingAssembly
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<Assembly>
                {
                    Success = false,
                    Message = $"Error updating assembly: {ex.Message}"
                };
            }
        }

        // ✅ DELETE ASSEMBLY
        public async Task<DbResponse<bool>> DeleteAssemblyAsync(int assemblyId)
        {
            try
            {
                var assembly = await _context.Assemblies.FindAsync(assemblyId);

                if (assembly == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Assembly not found"
                    };
                }

                _context.Assemblies.Remove(assembly);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Assembly deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting assembly: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<AssemblyLeader>>> GetAssemblyLeadersAsync()
        {
            try
            {
                if (_context == null)
                {
                    return new DbResponse<List<AssemblyLeader>>
                    {
                        Success = false,
                        Message = "Database context is not initialized"
                    };
                }

                var leaders = await _context.AssembliesLeaders
                    .AsNoTracking()
                    .Where(l => l.IsActive)  
                    .Include(l => l.Member)   
                    .Include(l => l.Assembly) 
                    .OrderByDescending(l => l.StartDate)
                    .ToListAsync();

                return new DbResponse<List<AssemblyLeader>>
                {
                    Success = true,
                    Data = leaders
                };
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("closed"))
            {
                Loggers.DoLogs(ex.Message);
                return new DbResponse<List<AssemblyLeader>>
                {
                    Success = false,
                    Message = "Database connection was closed. Please try again."
                };
            }
            catch (Exception ex)
            {
                //Loggers.DoLogs()
                return new DbResponse<List<AssemblyLeader>>
                {
                    Success = false,
                    Message = $"Error fetching assembly leaders: {ex.Message}"
                };
            }
        }

        // ✅ CREATE ASSEMBLY LEADER
        public async Task<DbResponse<AssemblyLeader>> CreateAssemblyLeaderAsync(AssemblyLeaderDto dto)
        {
            try
            {
                var memberExists = await _context.Members.AnyAsync(x => x.Id == dto.MemberId);
                if (!memberExists)
                {
                    return new DbResponse<AssemblyLeader>
                    {
                        Success = false,
                        Message = "Member not found"
                    };
                }

                var assemblyExists = await _context.Assemblies.AnyAsync(x => x.Id == dto.AssemblyId);
                if (!assemblyExists)
                {
                    return new DbResponse<AssemblyLeader>
                    {
                        Success = false,
                        Message = "Assembly not found"
                    };
                }

                var leader = new AssemblyLeader
                {
                    MemberId = dto.MemberId,
                    AssemblyId = dto.AssemblyId,
                    Bio = dto.Bio,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                _context.AssembliesLeaders.Add(leader);
                await _context.SaveChangesAsync();

                return new DbResponse<AssemblyLeader>
                {
                    Success = true,
                    Message = "Assembly leader assigned successfully",
                    Data = leader
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"CreateAssemblyLeaderAsync Error: {ex}");
                return new DbResponse<AssemblyLeader>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // ✅ GET ASSEMBLY LEADER BY ID
        public async Task<DbResponse<AssemblyLeader>> GetAssemblyLeaderByIdAsync(int id)
        {
            try
            {
                var leader = await _context.AssembliesLeaders
                    .Include(l => l.Member)
                    .Include(l => l.Assembly)
                    .FirstOrDefaultAsync(l => l.AssemblyLeaderId == id);

                if (leader == null)
                {
                    return new DbResponse<AssemblyLeader>
                    {
                        Success = false,
                        Message = "Assembly leader not found"
                    };
                }

                return new DbResponse<AssemblyLeader>
                {
                    Success = true,
                    Data = leader
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<AssemblyLeader>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // ✅ UPDATE ASSEMBLY LEADER
        public async Task<DbResponse<AssemblyLeader>> UpdateAssemblyLeaderAsync(int id, AssemblyLeaderDto dto)
        {
            try
            {
                var existing = await _context.AssembliesLeaders.FindAsync(id);
                if (existing == null)
                {
                    return new DbResponse<AssemblyLeader>
                    {
                        Success = false,
                        Message = "Assembly leader not found"
                    };
                }

                var memberExists = await _context.Members.AnyAsync(x => x.Id == dto.MemberId);
                if (!memberExists)
                {
                    return new DbResponse<AssemblyLeader>
                    {
                        Success = false,
                        Message = "Member not found"
                    };
                }

                var assemblyExists = await _context.Assemblies.AnyAsync(x => x.Id == dto.AssemblyId);
                if (!assemblyExists)
                {
                    return new DbResponse<AssemblyLeader>
                    {
                        Success = false,
                        Message = "Assembly not found"
                    };
                }

                existing.MemberId = dto.MemberId;
                existing.AssemblyId = dto.AssemblyId;
                existing.Bio = dto.Bio;
                existing.StartDate = dto.StartDate;
                existing.EndDate = dto.EndDate;
                existing.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                return new DbResponse<AssemblyLeader>
                {
                    Success = true,
                    Message = "Assembly leader updated successfully",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"UpdateAssemblyLeaderAsync Error: {ex}");
                return new DbResponse<AssemblyLeader>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // ✅ DELETE ASSEMBLY LEADER (SOFT)
        public async Task<DbResponse<bool>> DeleteAssemblyLeaderAsync(int id)
        {
            try
            {
                var leader = await _context.AssembliesLeaders.FindAsync(id);
                if (leader == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Assembly leader not found"
                    };
                }

                leader.IsActive = false;
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Assembly leader deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"DeleteAssemblyLeaderAsync Error: {ex}");
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // ✅ TOGGLE STATUS
        public async Task<DbResponse<bool>> ToggleAssemblyLeaderStatusAsync(int id, bool isActive)
        {
            try
            {
                var leader = await _context.AssembliesLeaders.FindAsync(id);
                if (leader == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Assembly leader not found"
                    };
                }

                leader.IsActive = isActive;
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = isActive ? "Assembly leader activated successfully" : "Assembly leader deactivated successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"ToggleAssemblyLeaderStatusAsync Error: {ex}");
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
