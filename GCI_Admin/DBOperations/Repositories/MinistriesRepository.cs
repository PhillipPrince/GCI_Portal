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

        // Add this method to your MinistriesRepository class

        // ✅ CREATE MINISTRY LEADER
        public async Task<DbResponse<MinistryLeader>> CreateMinistryLeaderAsync(MinistryLeaderDto dto)
        {
            try
            {
                // Validate required fields
                if (dto == null)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Ministry leader data is required"
                    };
                }

                if (dto.MemberId <= 0)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Valid Member ID is required"
                    };
                }

                if (dto.MinistryId <= 0)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Valid Ministry ID is required"
                    };
                }

                if (string.IsNullOrWhiteSpace(dto.PositionTitle))
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Position title is required"
                    };
                }

                // Check if member exists
                var memberExists = await _context.Members.AnyAsync(m => m.Id == dto.MemberId);
                if (!memberExists)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Selected member does not exist"
                    };
                }

                // Check if ministry exists
                var ministryExists = await _context.Ministries.AnyAsync(m => m.MinistryId == dto.MinistryId);
                if (!ministryExists)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Selected ministry does not exist"
                    };
                }

                // Check for duplicate active leadership (same member, same ministry, active)
                var existingActiveLeader = await _context.MinistryLeaders
                    .AnyAsync(ml => ml.MemberId == dto.MemberId
                                 && ml.MinistryId == dto.MinistryId
                                 && ml.IsActive);

                if (existingActiveLeader)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "This member is already an active leader for this ministry"
                    };
                }

                // Validate date range if EndDate is provided
                if (dto.EndDate.HasValue && dto.StartDate.HasValue && dto.EndDate.Value < dto.StartDate.Value)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "End date cannot be earlier than start date"
                    };
                }

                // Check for overlapping leadership periods for the same member in the same ministry
                if (dto.StartDate.HasValue)
                {
                    var overlappingLeader = await _context.MinistryLeaders
                        .AnyAsync(ml => ml.MemberId == dto.MemberId
                                     && ml.MinistryId == dto.MinistryId
                                     && ml.IsActive == false // Check inactive leaders for date overlap
                                     && ((dto.StartDate.Value >= ml.StartDate && dto.StartDate.Value <= (ml.EndDate ?? DateTime.MaxValue)) ||
                                         (dto.EndDate.HasValue && dto.EndDate.Value >= ml.StartDate && dto.EndDate.Value <= (ml.EndDate ?? DateTime.MaxValue)) ||
                                         (dto.StartDate.Value <= ml.StartDate && (!dto.EndDate.HasValue || dto.EndDate.Value >= ml.StartDate))));

                    if (overlappingLeader)
                    {
                        return new DbResponse<MinistryLeader>
                        {
                            Success = false,
                            Message = "This member already has a leadership role in this ministry during the specified period"
                        };
                    }
                }

                // Create new ministry leader
                var ministryLeader = new MinistryLeader
                {
                    MemberId = dto.MemberId,
                    MinistryId = dto.MinistryId,
                    PositionTitle = dto.PositionTitle.Trim(),
                    Bio = dto.Bio?.Trim() ?? string.Empty,
                    StartDate = dto.StartDate ?? DateTime.UtcNow,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Add to database
                await _context.MinistryLeaders.AddAsync(ministryLeader);
                await _context.SaveChangesAsync();

                // Load related data for the response
                await _context.Entry(ministryLeader)
                    .Reference(ml => ml.Member)
                    .LoadAsync();

                await _context.Entry(ministryLeader)
                    .Reference(ml => ml.Ministry)
                    .LoadAsync();

                // Log success
                Loggers.DoLogs($"Ministry Leader created successfully: MemberId={dto.MemberId}, MinistryId={dto.MinistryId}, Position={dto.PositionTitle}");

                return new DbResponse<MinistryLeader>
                {
                    Success = true,
                    Message = "Ministry leader assigned successfully",
                    Data = ministryLeader
                };
            }
            catch (DbUpdateException dbEx)
            {
                Loggers.DoLogs($"Database error in CreateMinistryLeaderAsync: {dbEx}");

                // Check for foreign key violations
                if (dbEx.InnerException?.Message.Contains("FOREIGN KEY") == true)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Invalid member or ministry reference. Please ensure both exist."
                    };
                }

                return new DbResponse<MinistryLeader>
                {
                    Success = false,
                    Message = $"Database error while creating ministry leader: {dbEx.Message}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in CreateMinistryLeaderAsync: {ex}");
                return new DbResponse<MinistryLeader>
                {
                    Success = false,
                    Message = $"Error creating ministry leader: {ex.Message}"
                };
            }
        }

        // ✅ UPDATE MINISTRY LEADER
        public async Task<DbResponse<MinistryLeader>> UpdateMinistryLeaderAsync(int ministryLeaderId, MinistryLeaderDto dto)
        {
            try
            {
                // Validate input
                if (dto == null)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Ministry leader data is required"
                    };
                }

                // Find existing ministry leader
                var ministryLeader = await _context.MinistryLeaders
                    .Include(ml => ml.Member)
                    .Include(ml => ml.Ministry)
                    .FirstOrDefaultAsync(ml => ml.MinistryLeaderId == ministryLeaderId);

                if (ministryLeader == null)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Ministry leader not found"
                    };
                }

                // Validate required fields
                if (dto.MemberId <= 0)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Valid Member ID is required"
                    };
                }

                if (dto.MinistryId <= 0)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Valid Ministry ID is required"
                    };
                }

                if (string.IsNullOrWhiteSpace(dto.PositionTitle))
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Position title is required"
                    };
                }

                // Check if member exists (if changed)
                if (ministryLeader.MemberId != dto.MemberId)
                {
                    var memberExists = await _context.Members.AnyAsync(m => m.Id == dto.MemberId);
                    if (!memberExists)
                    {
                        return new DbResponse<MinistryLeader>
                        {
                            Success = false,
                            Message = "Selected member does not exist"
                        };
                    }
                }

                // Check if ministry exists (if changed)
                if (ministryLeader.MinistryId != dto.MinistryId)
                {
                    var ministryExists = await _context.Ministries.AnyAsync(m => m.MinistryId == dto.MinistryId);
                    if (!ministryExists)
                    {
                        return new DbResponse<MinistryLeader>
                        {
                            Success = false,
                            Message = "Selected ministry does not exist"
                        };
                    }
                }

                // Check for duplicate active leadership (excluding current record)
                var existingActiveLeader = await _context.MinistryLeaders
                    .AnyAsync(ml => ml.MemberId == dto.MemberId
                                 && ml.MinistryId == dto.MinistryId
                                 && ml.IsActive
                                 && ml.MinistryLeaderId != ministryLeaderId);

                if (existingActiveLeader)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "This member is already an active leader for this ministry"
                    };
                }

                // Validate date range
                if (dto.EndDate.HasValue && dto.StartDate.HasValue && dto.EndDate.Value < dto.StartDate.Value)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "End date cannot be earlier than start date"
                    };
                }

                // Update properties
                ministryLeader.MemberId = dto.MemberId;
                ministryLeader.MinistryId = dto.MinistryId;
                ministryLeader.PositionTitle = dto.PositionTitle.Trim();
                ministryLeader.Bio = dto.Bio?.Trim() ?? string.Empty;
                ministryLeader.StartDate = dto.StartDate ?? DateTime.UtcNow;
                ministryLeader.EndDate = dto.EndDate;
                ministryLeader.IsActive = dto.IsActive;
                ministryLeader.UpdatedAt = DateTime.UtcNow;

                // If marking as inactive, set EndDate if not already set
                if (!dto.IsActive && !ministryLeader.EndDate.HasValue)
                {
                    ministryLeader.EndDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // Reload related data
                await _context.Entry(ministryLeader)
                    .Reference(ml => ml.Member)
                    .LoadAsync();

                await _context.Entry(ministryLeader)
                    .Reference(ml => ml.Ministry)
                    .LoadAsync();

                Loggers.DoLogs($"Ministry Leader updated successfully: Id={ministryLeaderId}");

                return new DbResponse<MinistryLeader>
                {
                    Success = true,
                    Message = "Ministry leader updated successfully",
                    Data = ministryLeader
                };
            }
            catch (DbUpdateException dbEx)
            {
                Loggers.DoLogs($"Database error in UpdateMinistryLeaderAsync: {dbEx}");
                return new DbResponse<MinistryLeader>
                {
                    Success = false,
                    Message = $"Database error while updating ministry leader: {dbEx.Message}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in UpdateMinistryLeaderAsync: {ex}");
                return new DbResponse<MinistryLeader>
                {
                    Success = false,
                    Message = $"Error updating ministry leader: {ex.Message}"
                };
            }
        }

        // ✅ GET MINISTRY LEADER BY ID
        public async Task<DbResponse<MinistryLeader>> GetMinistryLeaderByIdAsync(int ministryLeaderId)
        {
            try
            {
                var ministryLeader = await _context.MinistryLeaders
                    .AsNoTracking()
                    .Include(ml => ml.Member)
                    .Include(ml => ml.Ministry)
                    .FirstOrDefaultAsync(ml => ml.MinistryLeaderId == ministryLeaderId);

                if (ministryLeader == null)
                {
                    return new DbResponse<MinistryLeader>
                    {
                        Success = false,
                        Message = "Ministry leader not found"
                    };
                }

                return new DbResponse<MinistryLeader>
                {
                    Success = true,
                    Data = ministryLeader
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in GetMinistryLeaderByIdAsync: {ex}");
                return new DbResponse<MinistryLeader>
                {
                    Success = false,
                    Message = $"Error fetching ministry leader: {ex.Message}"
                };
            }
        }

        // ✅ GET MINISTRY LEADERS BY MINISTRY
        public async Task<DbResponse<List<MinistryLeader>>> GetMinistryLeadersByMinistryAsync(int ministryId)
        {
            try
            {
                var leaders = await _context.MinistryLeaders
                    .AsNoTracking()
                    .Where(ml => ml.MinistryId == ministryId)
                    .Include(ml => ml.Member)
                    .Include(ml => ml.Ministry)
                    .OrderByDescending(ml => ml.StartDate)
                    .ToListAsync();

                return new DbResponse<List<MinistryLeader>>
                {
                    Success = true,
                    Data = leaders
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in GetMinistryLeadersByMinistryAsync: {ex}");
                return new DbResponse<List<MinistryLeader>>
                {
                    Success = false,
                    Message = $"Error fetching ministry leaders: {ex.Message}"
                };
            }
        }

        // ✅ DELETE MINISTRY LEADER
        public async Task<DbResponse<bool>> DeleteMinistryLeaderAsync(int ministryLeaderId)
        {
            try
            {
                var ministryLeader = await _context.MinistryLeaders
                    .FirstOrDefaultAsync(ml => ml.MinistryLeaderId == ministryLeaderId);

                if (ministryLeader == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Ministry leader not found"
                    };
                }

                ministryLeader.IsActive = false;
                await _context.SaveChangesAsync();

                Loggers.DoLogs($"Ministry Leader soft deleted successfully: Id={ministryLeaderId}");

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Ministry leader deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in DeleteMinistryLeaderAsync: {ex}");
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting ministry leader: {ex.Message}"
                };
            }
        }

        // ✅ TOGGLE STATUS
        public async Task<DbResponse<bool>> ToggleMinistryLeaderStatusAsync(int id, bool isActive)
        {
            try
            {
                var leader = await _context.MinistryLeaders.FindAsync(id);
                if (leader == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Ministry leader not found"
                    };
                }

                leader.IsActive = isActive;
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = isActive ? "Ministry leader activated successfully" : "Ministry leader deactivated successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"ToggleMinistryLeaderStatusAsync Error: {ex}");
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // ✅ GET ACTIVE MINISTRY LEADERS
        public async Task<DbResponse<List<MinistryLeader>>> GetActiveMinistryLeadersAsync()
        {
            try
            {
                var leaders = await _context.MinistryLeaders
                    .AsNoTracking()
                    .Where(ml => ml.IsActive && (!ml.EndDate.HasValue || ml.EndDate.Value >= DateTime.UtcNow))
                    .Include(ml => ml.Member)
                    .Include(ml => ml.Ministry)
                    .OrderBy(ml => ml.Ministry.MinistryName)
                    .ThenBy(ml => ml.PositionTitle)
                    .ToListAsync();

                return new DbResponse<List<MinistryLeader>>
                {
                    Success = true,
                    Data = leaders
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in GetActiveMinistryLeadersAsync: {ex}");
                return new DbResponse<List<MinistryLeader>>
                {
                    Success = false,
                    Message = $"Error fetching active ministry leaders: {ex.Message}"
                };
            }
        }

        // ✅ GET MINISTRY MEMBERS
        public async Task<DbResponse<List<MinistryMember>>> GetMinistryMembersAsync(int ministryId)
        {
            try
            {
                var members = await _context.MinistryMembers
                    .Include(m => m.Member)
                    .Where(m => m.MinistryId == ministryId)
                    .ToListAsync();

                return new DbResponse<List<MinistryMember>>
                {
                    Success = true,
                    Data = members
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in GetMinistryMembersAsync: {ex}");
                return new DbResponse<List<MinistryMember>>
                {
                    Success = false,
                    Message = $"Error fetching ministry members: {ex.Message}"
                };
            }
        }

        // ✅ ADD MEMBER TO MINISTRY
        public async Task<DbResponse<bool>> AddMemberToMinistryAsync(MinistryMember newMember)
        {
            try
            {
                bool exists = await _context.MinistryMembers
                    .AnyAsync(m => m.MinistryId == newMember.MinistryId && m.MemberId == newMember.MemberId);

                if (exists)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Member is already in this Ministry."
                    };
                }

                _context.MinistryMembers.Add(newMember);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Member added successfully.",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in AddMemberToMinistryAsync: {ex}");
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error adding member: {ex.Message}"
                };
            }
        }
    }
}