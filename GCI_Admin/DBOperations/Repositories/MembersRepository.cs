using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class MembersRepository
    {
        private readonly AppDbContext _context;
        private readonly Security _security = new Security();


        public MembersRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DbResponse<List<Member>>> GetAllMembersAsync()
        {
            try
            {
                var members = await _context.Members
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<Member>>
                {
                    Success = true,
                    Data = members
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching members: {ex.ToString()}");
                return new DbResponse<List<Member>>
                {
                    Success = false,
                    Message = $"Error fetching members: {ex.Message}"
                };
            }
        }
        public async Task<DbResponse<Member>> GetMemberByIdAsync(int id)
        {
            try
            {
                var member = await _context.Members.FindAsync(id);
                if (member == null)
                {
                    return new DbResponse<Member>
                    {
                        Success = false,
                        Message = "Member not found"
                    };
                }
                return new DbResponse<Member>
                {
                    Success = true,
                    Data = member
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching member with ID {id}: {ex.Message}");
                return new DbResponse<Member>
                {
                    Success = false,
                    Message = $"Error fetching member: {ex.Message}"
                };
            }
        }


        public async Task<DbResponse<Member>> UpdateMemberAsync(int id, MemberDto dto)
        {
            try
            {
                var existingMember = await _context.Members.FindAsync(id);

                if (existingMember == null)
                {
                    return new DbResponse<Member>
                    {
                        Success = false,
                        Message = "Member not found"
                    };
                }

                existingMember.FirstName = dto.FirstName;
                existingMember.OtherNames = dto.OtherNames;
                existingMember.Phone = dto.Phone;
                existingMember.Email = dto.Email;
                existingMember.Gender = dto.Gender;
                existingMember.Assembly = dto.Assembly;
                existingMember.StatusId = dto.StatusId;

                await _context.SaveChangesAsync();

                return new DbResponse<Member>
                {
                    Success = true,
                    Message = "Member updated successfully",
                    Data = existingMember
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error updating member with ID {id}: {ex.Message}");
                return new DbResponse<Member>
                {
                    Success = false,
                    Message = $"Error updating member: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<bool>> DeleteMemberAsync(int id)
        {
            try
            {
                var member = await _context.Members.FindAsync(id);

                if (member == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Member not found"
                    };
                }

                _context.Members.Remove(member);
                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Member deleted successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error deleting member with ID {id}: {ex.Message}");
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting member: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<Member>> CreateUserAsync(MemberDto dto)
        {
            try
            {
                bool exists = await _context.Members.AnyAsync(x =>
                    x.Phone == dto.Phone || x.Email == dto.Email);

                if (exists)
                    return new DbResponse<Member>
                    {
                        Success = false,
                        Message = "Phone or Email already exists."
                    };

                var user = new Member
                {
                    FirstName = dto.FirstName,
                    OtherNames = dto.OtherNames,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Gender = dto.Gender,
                    Assembly = dto.Assembly,

                    SocialMediaName = dto.SocialMediaName,
                    ResidentialAddress = dto.ResidentialAddress,
                    DateOfBirth = dto.DateOfBirth,
                    MaritalStatus = dto.MaritalStatus,
                    NumberOfChildren = dto.NumberOfChildren,
                    SpouseName = dto.SpouseName,

                    PasswordHash = _security.EncryptStringAES("Password1234", "GCI"),
                    CreatedAt = DateTime.Now,
                    StatusId = 3
                };


                _context.Members.Add(user);
                await _context.SaveChangesAsync();

                return new DbResponse<Member>
                {
                    Success = true,
                    Message = "User created successfully.",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UserRepository->CreateUserAsync->" + ex.Message);
                return new DbResponse<Member>
                {
                    Success = false,
                    Message = "An error occurred while creating the user."
                };
            }
        }
        public async Task<DbResponse<MembershipClass>> CreateMembershipClassAsync(MembershipClassDto dto)
        {
            try
            {
                // Prevent duplicate membership class for same member and year
                bool exists = await _context.MembershipClasses.AnyAsync(x =>
                    x.MemberId == dto.MemberId &&
                    x.MembershipYear == dto.MembershipYear &&
                    x.IsActive);

                if (exists)
                    return new DbResponse<MembershipClass>
                    {
                        Success = false,
                        Message = "Membership class already exists for this member and year."
                    };

                var membership = new MembershipClass
                {
                    MemberId = dto.MemberId,
                    MembershipYear = dto.MembershipYear,
                    Cohort = dto.Cohort,
                    IsMemberOfAnotherChurch = dto.IsMemberOfAnotherChurch,
                    FormerChurchName = dto.FormerChurchName,
                    ReasonForLeavingFormerChurch = dto.ReasonForLeavingFormerChurch,
                    DateBeganAttendingGCI = dto.DateBeganAttendingGCI,
                    SeekingMembership = dto.SeekingMembership,
                    IsBornAgain = dto.IsBornAgain,
                    DateOfConversion = dto.DateOfConversion,
                    PlaceOfConversion = dto.PlaceOfConversion,
                    HasEternalLifeAssurance = dto.HasEternalLifeAssurance,
                    HeavenReason = dto.HeavenReason,
                    MeaningOfChristsDeath = dto.MeaningOfChristsDeath,
                    IsBaptizedByImmersion = dto.IsBaptizedByImmersion,
                    BaptismDate = dto.BaptismDate,
                    BaptismPlace = dto.BaptismPlace,
                    WillingToBeBaptizedAtGCI = dto.WillingToBeBaptizedAtGCI,
                    PreviousMinistryExperience = dto.PreviousMinistryExperience,
                    SpecialGiftsOrServiceInterest = dto.SpecialGiftsOrServiceInterest,
                    IsInformationConfirmed = dto.IsInformationConfirmed,

                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.MembershipClasses.Add(membership);
                await _context.SaveChangesAsync();

                return new DbResponse<MembershipClass>
                {
                    Success = true,
                    Message = "Membership class created successfully.",
                    Data = membership
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("MembershipClassRepository->CreateMembershipClassAsync->" + ex.Message);

                return new DbResponse<MembershipClass>
                {
                    Success = false,
                    Message = "An error occurred while creating the membership class."
                };
            }
        }



    }
}
