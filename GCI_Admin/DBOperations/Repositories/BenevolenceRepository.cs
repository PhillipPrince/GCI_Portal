using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class BenevolenceRepository
    {
        private readonly AppDbContext _context;

        public BenevolenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse<List<BenevolenceMember>>> GetAllBenevolenceMembersAsync()
        {
            try
            {

                _context.ChangeTracker.Clear();   // Clears EF tracking

                var members = await _context.BenevolenceMembers
                    .AsNoTracking()
                    .Include(m => m.Member)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<BenevolenceMember>>
                {
                    Success = true,
                    Data = members
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<BenevolenceMember>>
                {
                    Success = false,
                    Message = $"Error fetching benevolence members: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<BenevolenceBeneficiary>>> GetBenevolenceBeneficiariesAsync(int beneId)
        {
            try
            {
                _context.ChangeTracker.Clear();

                var beneficiaries = await _context.BenevolenceBeneficiaries
                    .AsNoTracking()
                    .Where(b => b.BenevolenceMemberId == beneId)
                    .OrderByDescending(b => b.Id)
                    .ToListAsync();

                return new DbResponse<List<BenevolenceBeneficiary>>
                {
                    Success = true,
                    Data = beneficiaries
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<BenevolenceBeneficiary>>
                {
                    Success = false,
                    Message = $"Error fetching benevolence beneficiaries: {ex.Message}"
                };
            }
        }
        public async Task<DbResponse<BenevolenceMember>> GetBenevolenceMemberByIdAsync(int id)
        {
            try
            {
                var member = await _context.BenevolenceMembers
                    .Include(m => m.Member)
                    .FirstOrDefaultAsync(m => m.Id == id);

                return new DbResponse<BenevolenceMember>
                {
                    Success = true,
                    Data = member
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<BenevolenceMember>
                {
                    Success = false,
                    Message = $"Error fetching member: {ex.Message}"
                };
            }
        }

        //public async Task<DbResponse<BenevolenceMember>> CreateBenevolenceMemberAsync(BenevolenceMemberDto dto)
        //{
        //    try
        //    {
        //        var entity = new BenevolenceMember
        //        {
        //            MemberId = dto.MemberId,
        //            PreferredCoverAmount = dto.PreferredCoverAmount,
        //            NationalId = dto.NationalId,
        //            NextOfKinName = dto.NextOfKinName,
        //            NextOfKinPhone = dto.NextOfKinPhone,
        //            NumberOfDependants = dto.NumberOfDependants,
        //            RegNo = dto.RegNo,
        //            TotalAmountDue = dto.TotalAmountDue,
        //            AmountPaid = dto.AmountPaid,
        //            BalanceAmount = dto.TotalAmountDue - dto.AmountPaid,
        //            IsActive = true,
        //            CreatedAt = DateTime.Now
        //        };

        //        _context.BenevolenceMembers.Add(entity);
        //        await _context.SaveChangesAsync();

        //        return new DbResponse<BenevolenceMember>
        //        {
        //            Success = true,
        //            Data = entity
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DbResponse<BenevolenceMember>
        //        {
        //            Success = false,
        //            Message = $"Error creating member: {ex.Message}"
        //        };
        //    }
        //}

        //public async Task<DbResponse<BenevolenceMember>> UpdateBenevolenceMemberAsync(int id, BenevolenceMemberDto dto)
        //{
        //    try
        //    {
        //        var member = await _context.BenevolenceMembers.FindAsync(id);

        //        if (member == null)
        //        {
        //            return new DbResponse<BenevolenceMember>
        //            {
        //                Success = false,
        //                Message = "Member not found"
        //            };
        //        }

        //        member.MemberId = dto.MemberId;
        //        member.PreferredCoverAmount = dto.PreferredCoverAmount;
        //        member.NationalId = dto.NationalId;
        //        member.NextOfKinName = dto.NextOfKinName;
        //        member.NextOfKinPhone = dto.NextOfKinPhone;
        //        member.NumberOfDependants = dto.NumberOfDependants;
        //        member.RegNo = dto.RegNo;
        //        member.TotalAmountDue = dto.TotalAmountDue;
        //        member.AmountPaid = dto.AmountPaid;
        //        member.BalanceAmount = dto.TotalAmountDue - dto.AmountPaid;
        //        member.UpdatedAt = DateTime.Now;

        //        await _context.SaveChangesAsync();

        //        return new DbResponse<BenevolenceMember>
        //        {
        //            Success = true,
        //            Data = member
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new DbResponse<BenevolenceMember>
        //        {
        //            Success = false,
        //            Message = $"Error updating member: {ex.Message}"
        //        };
        //    }
        //}

        public async Task<DbResponse<bool>> DeleteBenevolenceMemberAsync(int id)
        {
            try
            {
                var member = await _context.BenevolenceMembers.FindAsync(id);

                if (member == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Member not found"
                    };
                }

                _context.BenevolenceMembers.Remove(member);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = "Member deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Error deleting member: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<bool>> ToggleBenevolenceMemberStatusAsync(int id, bool isActive)
        {
            try
            {
                var member = await _context.BenevolenceMembers.FindAsync(id);

                if (member == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Data = false,
                        Message = "Member not found"
                    };
                }

                member.IsActive = isActive;
                member.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Data = true,
                    Message = isActive ? "Member activated successfully" : "Member deactivated successfully"
                };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Error updating status: {ex.Message}"
                };
            }
        }
    }
}