using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class GECMemberRepository
    {
        private readonly AppDbContext _context;

        public GECMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse<GECMember>> CreateGECMemberAsync(GECMemberDto dto)
        {
            try
            {
                var newMember = new GECMember
                {
                    MemberId = dto.MemberId,
                    PositionTitle = dto.PositionTitle,
                    Bio = dto.Bio,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now
                };

                _context.GECMembers.Add(newMember);
                await _context.SaveChangesAsync();

                return new DbResponse<GECMember>
                {
                    Success = true,
                    Message = "GEC member created successfully",
                    Data = newMember
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in CreateGECMemberAsync: {ex}");
                return new DbResponse<GECMember>
                {
                    Success = false,
                    Message = $"Error creating GEC member: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<GECMember>>> GetGECMembersAsync()
        {
            try
            {
                var gecMembers = await _context.GECMembers
                    .Where(g => g.IsActive)
                    .OrderBy(g => g.GECId)
                    .ToListAsync();

                return new DbResponse<List<GECMember>>
                {
                    Success = true,
                    Data = gecMembers
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<GECMember>>
                {
                    Success = false,
                    Message = $"Error fetching GEC members: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<GECMember>> GetGECMemberByIdAsync(int gecId)
        {
            try
            {
                var member = await _context.GECMembers
                    .FirstOrDefaultAsync(x => x.GECId == gecId);

                if (member == null)
                {
                    return new DbResponse<GECMember>
                    {
                        Success = false,
                        Message = "GEC member not found"
                    };
                }

                return new DbResponse<GECMember>
                {
                    Success = true,
                    Data = member
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<GECMember>
                {
                    Success = false,
                    Message = $"Error fetching GEC member: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<GECMember>> UpdateGECMemberAsync(int gecId, GECMemberDto dto)
        {
            try
            {
                var existingMember = await _context.GECMembers.FindAsync(gecId);

                if (existingMember == null)
                {
                    return new DbResponse<GECMember>
                    {
                        Success = false,
                        Message = "GEC member not found"
                    };
                }

                existingMember.MemberId = dto.MemberId;
                existingMember.PositionTitle = dto.PositionTitle;
                existingMember.Bio = dto.Bio;
                existingMember.StartDate = dto.StartDate;
                existingMember.EndDate = dto.EndDate;
                existingMember.IsActive = dto.IsActive;

                await _context.SaveChangesAsync();

                return new DbResponse<GECMember>
                {
                    Success = true,
                    Message = "GEC member updated successfully",
                    Data = existingMember
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in UpdateGECMemberAsync: {ex}");
                return new DbResponse<GECMember>
                {
                    Success = false,
                    Message = $"Error updating GEC member: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<bool>> DeleteGECMemberAsync(int gecId)
        {
            try
            {
                var member = await _context.GECMembers.FindAsync(gecId);

                if (member == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "GEC member not found"
                    };
                }

                _context.GECMembers.Remove(member);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "GEC member deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting GEC member: {ex.Message}"
                };
            }
        }
    }
}
