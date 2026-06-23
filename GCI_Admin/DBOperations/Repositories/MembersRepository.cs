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

        public async Task<DbResponse<DataTableResponse<Member>>> GetMembersDataTableAsync(int draw, int start, int length, string searchValue, int? statusId)
        {
            try
            {
                var query = _context.Members.AsQueryable();

                if (statusId.HasValue && statusId.Value > 0)
                {
                    query = query.Where(m => m.StatusId == statusId.Value);
                }

                int recordsTotal = await query.CountAsync();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.ToLower();
                    query = query.Where(m => 
                        (m.FirstName != null && m.FirstName.ToLower().Contains(searchValue)) ||
                        (m.OtherNames != null && m.OtherNames.ToLower().Contains(searchValue)) ||
                        (m.Phone != null && m.Phone.ToLower().Contains(searchValue)) ||
                        (m.Email != null && m.Email.ToLower().Contains(searchValue)) ||
                        (m.Assembly != null && m.Assembly.ToLower().Contains(searchValue))
                    );
                }

                int recordsFiltered = await query.CountAsync();

                var data = await query
                    .OrderByDescending(m => m.CreatedAt)
                    .Skip(start)
                    .Take(length)
                    .ToListAsync();

                return new DbResponse<DataTableResponse<Member>>
                {
                    Success = true,
                    Data = new DataTableResponse<Member>
                    {
                        draw = draw,
                        recordsTotal = recordsTotal,
                        recordsFiltered = recordsFiltered,
                        data = data
                    }
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in GetMembersDataTableAsync: {ex.Message}");
                return new DbResponse<DataTableResponse<Member>>
                {
                    Success = false,
                    Message = ex.Message
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
                    PasswordHash = _security.EncryptStringAES("Password@1234", "GCI"),
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

        public async Task<DbResponse<MemberAdditionalInformation>> CreateAdditionalInfoAsync(CreateMemberAdditionalInformationDto dto)
        {
            try
            {
                var member = await _context.Members.FindAsync(dto.MemberId);
                if (member == null)
                {
                    return new DbResponse<MemberAdditionalInformation>
                    {
                        Success = false,
                        Message = "Member not found."
                    };
                }

                bool exists = await _context.MemberAdditionalInformations
                    .AnyAsync(x => x.MemberId == dto.MemberId && x.IsActive);

                if (exists)
                    return new DbResponse<MemberAdditionalInformation>
                    {
                        Success = false,
                        Message = "Additional information already exists for this member."
                    };

                member.StatusId = 2;

                var entity = new MemberAdditionalInformation
                {
                    MemberId = dto.MemberId,
                    MembershipYear = int.Parse(dto.MembershipYear.ToString()),
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

                _context.MemberAdditionalInformations.Add(entity);
                await _context.SaveChangesAsync();

                return new DbResponse<MemberAdditionalInformation>
                {
                    Success = true,
                    Message = "Created successfully",
                    Data = entity
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("CreateAdditionalInfoAsync -> " + ex.Message);

                return new DbResponse<MemberAdditionalInformation>
                {
                    Success = false,
                    Message = "Error creating record"
                };
            }
        }
        public async Task<DbResponse<MemberAdditionalInformation>> GetAdditionalInfoByMemberIdAsync(int memberId)
        {
            try
            {
                var data = await _context.MemberAdditionalInformations
                    .FirstOrDefaultAsync(x => x.MemberId == memberId);

                if (data == null)
                    return new DbResponse<MemberAdditionalInformation>
                    {
                        Success = false,
                        Message = "Record not found"
                    };

                return new DbResponse<MemberAdditionalInformation>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("GetAdditionalInfoByMemberIdAsync -> " + ex.Message);

                return new DbResponse<MemberAdditionalInformation>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<DbResponse<MemberAdditionalInformation>> UpdateAdditionalInfoAsync(int id, MemberAdditionalInformationDto dto)
        {
            try
            {
                var existing = await _context.MemberAdditionalInformations.FindAsync(id);

                if (existing == null)
                    return new DbResponse<MemberAdditionalInformation>
                    {
                        Success = false,
                        Message = "Record not found"
                    };

                _context.Entry(existing).CurrentValues.SetValues(dto);
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new DbResponse<MemberAdditionalInformation>
                {
                    Success = true,
                    Message = "Updated successfully",
                    Data = existing
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UpdateAdditionalInfoAsync -> " + ex.Message);

                return new DbResponse<MemberAdditionalInformation>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<DbResponse<bool>> UpdateMemberRoleAsync(int memberId, int roleId)
        {
            try
            {
                var member = await _context.Members.FindAsync(memberId);

                if (member == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Member not found"
                    };
                }
                if (member.StatusId != 1)
                {
                    return new DbResponse<bool>

                    {
                        Success = false,
                        Message = "Member Not Fu"
                    };
                }

                member.UserRole = roleId;

                await _context.SaveChangesAsync();

                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Role updated successfully",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in UpdateMemberRoleAsync: {ex}");

                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating role: {ex.Message}"
                };
            }
        }
        public async Task<DbResponse<bool>> UpdateFullMembershipStatusAsync(int memberId)
        {
            try
            {
                var member = await _context.Members.FindAsync(memberId);
                if (member == null)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Member not found"
                    };
                }
                if (member.StatusId != 2)
                {
                    return new DbResponse<bool>
                    {
                        Success = false,
                        Message = "Member is not in Membership Class status"
                    };
                }
                member.StatusId = 1;
                await _context.SaveChangesAsync();
                return new DbResponse<bool>
                {
                    Success = true,
                    Message = "Membership status updated to Active Member",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error in UpdateFullMembershipStatusAsync: {ex}");
                return new DbResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating membership status: {ex.Message}"
                };
            }

        }
        // Add these methods to your MembersRepository class

        public async Task<DbResponse<List<Member>>> GetMembersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var members = await _context.Members
                    .Where(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<Member>>
                {
                    Success = true,
                    Data = members,
                    Message = $"Found {members.Count} members created between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching members by date range: {ex.ToString()}");
                return new DbResponse<List<Member>>
                {
                    Success = false,
                    Message = $"Error fetching members by date range: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<Member>>> GetActiveMembersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var members = await _context.Members
                    .Where(m => m.StatusId == 1 && m.CreatedAt >= startDate && m.CreatedAt <= endDate)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<Member>>
                {
                    Success = true,
                    Data = members,
                    Message = $"Found {members.Count} active members created between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching active members by date range: {ex.ToString()}");
                return new DbResponse<List<Member>>
                {
                    Success = false,
                    Message = $"Error fetching active members by date range: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<Member>>> GetFullMembersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var members = await _context.Members
                    .Where(m => m.StatusId == 1 && m.CreatedAt >= startDate && m.CreatedAt <= endDate)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<Member>>
                {
                    Success = true,
                    Data = members,
                    Message = $"Found {members.Count} full members created between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching full members by date range: {ex.ToString()}");
                return new DbResponse<List<Member>>
                {
                    Success = false,
                    Message = $"Error fetching full members by date range: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<List<Member>>> GetMembersByStatusAndDateRangeAsync(int statusId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var members = await _context.Members
                    .Where(m => m.StatusId == statusId && m.CreatedAt >= startDate && m.CreatedAt <= endDate)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<Member>>
                {
                    Success = true,
                    Data = members,
                    Message = $"Found {members.Count} members with status {statusId} created between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching members by status and date range: {ex.ToString()}");
                return new DbResponse<List<Member>>
                {
                    Success = false,
                    Message = $"Error fetching members by status and date range: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<int>> GetMembersCountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var count = await _context.Members
                    .CountAsync(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate);

                return new DbResponse<int>
                {
                    Success = true,
                    Data = count,
                    Message = $"Found {count} members created between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error counting members by date range: {ex.ToString()}");
                return new DbResponse<int>
                {
                    Success = false,
                    Message = $"Error counting members by date range: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<int>> GetActiveMembersCountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var count = await _context.Members
                    .CountAsync(m => m.StatusId == 1 && m.CreatedAt >= startDate && m.CreatedAt <= endDate);

                return new DbResponse<int>
                {
                    Success = true,
                    Data = count,
                    Message = $"Found {count} active members created between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error counting active members by date range: {ex.ToString()}");
                return new DbResponse<int>
                {
                    Success = false,
                    Message = $"Error counting active members by date range: {ex.Message}"
                };
            }
        }

        public async Task<DbResponse<Dictionary<DateTime, int>>> GetMembersGroupedByDateAsync(DateTime startDate, DateTime endDate, string groupBy = "day")
        {
            try
            {
                var query = _context.Members
                    .Where(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate);

                Dictionary<DateTime, int> groupedData = new Dictionary<DateTime, int>();

                if (groupBy.ToLower() == "day")
                {
                    groupedData = await query
                        .GroupBy(m => m.CreatedAt.Date)
                        .Select(g => new { Date = g.Key, Count = g.Count() })
                        .ToDictionaryAsync(g => g.Date, g => g.Count);
                }
                else if (groupBy.ToLower() == "week")
                {
                    groupedData = await query
                        .GroupBy(m => new {
                            Year = m.CreatedAt.Year,
                            Week = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                m.CreatedAt,
                                System.Globalization.CalendarWeekRule.FirstDay,
                                DayOfWeek.Sunday)
                        })
                        .Select(g => new {
                            Date = new DateTime(g.Key.Year, 1, 1).AddDays((g.Key.Week - 1) * 7),
                            Count = g.Count()
                        })
                        .ToDictionaryAsync(g => g.Date, g => g.Count);
                }
                else if (groupBy.ToLower() == "month")
                {
                    groupedData = await query
                        .GroupBy(m => new { m.CreatedAt.Year, m.CreatedAt.Month })
                        .Select(g => new { Date = new DateTime(g.Key.Year, g.Key.Month, 1), Count = g.Count() })
                        .ToDictionaryAsync(g => g.Date, g => g.Count);
                }
                else if (groupBy.ToLower() == "year")
                {
                    groupedData = await query
                        .GroupBy(m => m.CreatedAt.Year)
                        .Select(g => new { Date = new DateTime(g.Key, 1, 1), Count = g.Count() })
                        .ToDictionaryAsync(g => g.Date, g => g.Count);
                }

                return new DbResponse<Dictionary<DateTime, int>>
                {
                    Success = true,
                    Data = groupedData,
                    Message = $"Found {groupedData.Count} date groups"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching members grouped by date: {ex.ToString()}");
                return new DbResponse<Dictionary<DateTime, int>>
                {
                    Success = false,
                    Message = $"Error fetching members grouped by date: {ex.Message}"
                };
            }
        }

        // Optional: Method to get members who attained membership class within date range
        public async Task<DbResponse<List<Member>>> GetMembersInMembershipClassByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Get member IDs that have additional info created within the date range
                var memberIds = await _context.MemberAdditionalInformations
                    .Where(info => info.CreatedAt >= startDate && info.CreatedAt <= endDate)
                    .Select(info => info.MemberId)
                    .ToListAsync();

                var members = await _context.Members
                    .Where(m => memberIds.Contains(m.Id) && m.StatusId == 2)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return new DbResponse<List<Member>>
                {
                    Success = true,
                    Data = members,
                    Message = $"Found {members.Count} members in membership class between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}"
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Error fetching membership class members by date range: {ex.ToString()}");
                return new DbResponse<List<Member>>
                {
                    Success = false,
                    Message = $"Error fetching membership class members by date range: {ex.Message}"
                };
            }
        }
        public async Task<DbResponse<Member>> UpdateUserStatus(string phone, int statusId)
        {
            try
            {
                var user = await _context.Members.FirstOrDefaultAsync(x => x.Phone == phone);
                if (user == null)
                {
                    return new DbResponse<Member>
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }
                //var statusExists = await _context.Statuses
                //    .AnyAsync(x => x.Id == statusId);
                //if (!statusExists)
                //{
                //    return new DbResponse<User>
                //    {
                //        Success = false,
                //        Message = "Status not found."
                //    };
                //}
                user.StatusId = statusId;
                // user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return new DbResponse<Member>
                {
                    Success = true,
                    Message = "User status updated successfully.",
                    Data = user
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs("UserRepository->UpdateUserStatus->" + ex.Message);
                return new DbResponse<Member>
                {
                    Success = false,
                    Message = "An error occurred while updating user status."
                };
            }
        }

    }
}
