using ClosedXML.Excel;
using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.EntityFrameworkCore;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class MembersService : IMembersService
    {
        private readonly MembersRepository _membersRepository;
        private readonly AppDbContext _context;
        private readonly Security _security = new Security();



        public MembersService(MembersRepository membersRepository, AppDbContext context)
        {
            _membersRepository = membersRepository;
            _context = context;
        }

        // ✅ GET ALL MEMBERS
        public async Task<ApiResponse<List<Member>>> GetAllMembersAsync()
        {
            var response = new ApiResponse<List<Member>>();

            try
            {
                var result = await _membersRepository.GetAllMembersAsync();

                response.Data = result.Data;
                response.Message = "Members retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<ApiResponse<Member>> GetMemberByIdAsync(int id)
        {
            var response = new ApiResponse<Member>();
            try
            {
                var result = await _membersRepository.GetMemberByIdAsync(id);
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Member not found";
                    return response;
                }
                response.Data = result.Data;
                response.Message = "Member retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        // ✅ UPDATE MEMBER
        public async Task<ApiResponse<Member>> UpdateMemberAsync(int id, MemberDto dto)
        {
            var response = new ApiResponse<Member>();

            try
            {
                var result = await _membersRepository.UpdateMemberAsync(id, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Member not found or update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Member updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ DELETE MEMBER
        public async Task<ApiResponse<bool>> DeleteMemberAsync(int id)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _membersRepository.DeleteMemberAsync(id);

                if (!result.Success || !result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Member not found or delete failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = result.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ CREATE MEMBER
        public async Task<ApiResponse<Member>> CreateUserAsync(MemberDto dto)
        {
            var response = new ApiResponse<Member>();

            try
            {
                var result = await _membersRepository.CreateUserAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Failed to create member";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Member created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // =========================================================
        // 🔥 MEMBER ADDITIONAL INFORMATION (REPLACES MEMBERSHIP CLASS)
        // =========================================================

        // ✅ CREATE
        public async Task<ApiResponse<MemberAdditionalInformation>> CreateAdditionalInfoAsync(MemberAdditionalInformationDto dto)
        {
            var response = new ApiResponse<MemberAdditionalInformation>();

            try
            {
                var result = await _membersRepository.CreateAdditionalInfoAsync(dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Failed to create additional info";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Additional information created successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

      
        public async Task<ApiResponse<MemberAdditionalInformation>> GetAdditionalInfoByMemberIdAsync(int memberId)
        {
            var response = new ApiResponse<MemberAdditionalInformation>();

            try
            {
                var result = await _membersRepository.GetAdditionalInfoByMemberIdAsync(memberId);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message;
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Record retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        // ✅ UPDATE
        public async Task<ApiResponse<MemberAdditionalInformation>> UpdateAdditionalInfoAsync(int id, MemberAdditionalInformationDto dto)
        {
            var response = new ApiResponse<MemberAdditionalInformation>();

            try
            {
                var result = await _membersRepository.UpdateAdditionalInfoAsync(id, dto);

                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message ?? "Update failed";
                    return response;
                }

                response.Data = result.Data;
                response.Message = "Updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }
        // ✅ UPDATE MEMBER ROLE
        public async Task<ApiResponse<bool>> UpdateMemberRoleAsync(int memberId, int roleId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _membersRepository.UpdateMemberRoleAsync(memberId, roleId);

                if (!result.Success || !result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result.Message ?? "Member not found or role update failed";
                    return response;
                }

                response.Data = true;
                response.Message = "Member role updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<ApiResponse<MemberUploadResponse>> ProcessMembersExcelUploadAsync(
   IFormFile file, string createdBy, string uploadOption)
        {
            if (file == null || file.Length == 0)
            {
                return new ApiResponse<MemberUploadResponse>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = "No file uploaded"
                };
            }

            var extension = Path.GetExtension(file.FileName)?.ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return new ApiResponse<MemberUploadResponse>
                {
                    IsSuccess = false,
                    Code = "400",
                    Message = "Only Excel files are allowed"
                };
            }

            try
            {
                List<ExcelMemberDto> excelRows;

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    excelRows = ReadExcelMemberFile(stream);
                }

                if (excelRows == null || excelRows.Count == 0)
                {
                    return new ApiResponse<MemberUploadResponse>
                    {
                        IsSuccess = false,
                        Code = "400",
                        Message = "No valid data found"
                    };
                }

                var response = new MemberUploadResponse
                {
                    TotalRecords = excelRows.Count
                };

                // 🔥 Overwrite option
                if (uploadOption == "overwrite")
                {
                    var existing = _context.Members.ToList();
                    _context.Members.RemoveRange(existing);
                    await _context.SaveChangesAsync();
                }

                var membersToInsert = new List<Member>();

                foreach (var row in excelRows)
                {
                    try
                    {
                        // ✅ Required fields
                        if (string.IsNullOrWhiteSpace(row.FirstName) ||
                            string.IsNullOrWhiteSpace(row.Phone) ||
                            string.IsNullOrWhiteSpace(row.Email))
                        {
                            response.FailedRecords++;
                            response.ErrorMessages.Add($"Row {row.RowNumber}: Missing required fields.");
                            continue;
                        }

                        // ✅ Date parsing
                        DateTime? dob = null;
                        if (!string.IsNullOrWhiteSpace(row.DateOfBirth))
                        {
                            if (DateTime.TryParse(row.DateOfBirth, out DateTime parsedDob))
                                dob = parsedDob;
                            else
                            {
                                response.FailedRecords++;
                                response.ErrorMessages.Add($"Row {row.RowNumber}: Invalid DateOfBirth.");
                                continue;
                            }
                        }

                        // ✅ Number of children
                        int children = 0;
                        if (!string.IsNullOrWhiteSpace(row.NumberOfChildren))
                        {
                            int.TryParse(row.NumberOfChildren, out children);
                        }

                        // ✅ Normalize phone (Kenya format)
                        // Normalize safely
                        string phone = NormalizePhone(row?.Phone ?? "");
                        string email = row?.Email?.Trim() ?? "";

                        // Skip if both are empty (avoid useless query)
                        if (string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(email))
                        {
                            response.FailedRecords++;
                            response.ErrorMessages.Add($"Row {row?.RowNumber}: Phone and Email cannot both be empty.");
                            continue;
                        }

                        // Safe duplicate check
                        bool exists = await _context.Members
                            .AnyAsync(x =>
                                (!string.IsNullOrEmpty(phone) && x.Phone == phone) ||
                                (!string.IsNullOrEmpty(email) && x.Email == email)
                            );
                        if (exists && uploadOption != "overwrite")
                        {
                            response.FailedRecords++;
                            response.ErrorMessages.Add($"Row {row.RowNumber}: Duplicate member.");
                            continue;
                        }

                        var member = new Member
                        {
                            FirstName = row.FirstName,
                            OtherNames = row.OtherNames,
                            Phone = phone,
                            Email = row.Email,
                            Gender = row.Gender,
                            DateOfBirth = dob,
                            MaritalStatus = row.MaritalStatus,
                            SpouseName = row.SpouseName,
                            NumberOfChildren = children,
                            Assembly = row.Assembly,
                            ResidentialAddress = row.ResidentialAddress,
                            SocialMediaName = row.SocialMediaName,
                            CreatedAt = DateTime.Now,
                            MustChangePassword= true,
                            PasswordHash = _security.EncryptStringAES("Password@1234", "GCI"),
                            StatusId=1,
                            UserRole=3

                            //CreatedBy = createdBy
                        };

                        membersToInsert.Add(member);
                        response.SuccessfulRecords++;
                    }
                    catch (Exception ex)
                    {
                        Loggers.DoLogs($"Error processing row {row.RowNumber}: {ex.Message}");
                        response.FailedRecords++;
                        response.ErrorMessages.Add($"Row {row.RowNumber}: {ex.Message}");
                    }
                }

                if (membersToInsert.Any())
                {
                    await _context.Members.AddRangeAsync(membersToInsert);
                    await _context.SaveChangesAsync();
                }
                Loggers.EventLogs($"Excel upload completed: {response.TotalRecords} total, {response.SuccessfulRecords} successful, {response.FailedRecords} failed.");

                return new ApiResponse<MemberUploadResponse>
                {
                    IsSuccess = true,
                    Code = "200",
                    Message = "Upload completed successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<MemberUploadResponse>
                {
                    IsSuccess = false,
                    Code = "500",
                    Message = ex.Message
                };
            }
        }
        private List<ExcelMemberDto> ReadExcelMemberFile(Stream stream)
        {
            var rows = new List<ExcelMemberDto>();

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);

            var dataRows = worksheet.RowsUsed().Skip(1);
            int rowNumber = 2;

            foreach (var row in dataRows)
            {
                var item = new ExcelMemberDto
                {
                    RowNumber = rowNumber,
                    FirstName = GetCellValue(row.Cell(1)),
                    OtherNames = GetCellValue(row.Cell(2)),
                    Phone = GetCellValue(row.Cell(3)),
                    Email = GetCellValue(row.Cell(4)),
                    Gender = GetCellValue(row.Cell(5)),
                    DateOfBirth = GetCellValue(row.Cell(6)),
                    MaritalStatus = GetCellValue(row.Cell(7)),
                    SpouseName = GetCellValue(row.Cell(8)),
                    NumberOfChildren = GetCellValue(row.Cell(9)),
                    Assembly = GetCellValue(row.Cell(10)),
                    ResidentialAddress = GetCellValue(row.Cell(11)),
                    SocialMediaName = GetCellValue(row.Cell(12))
                };

                if (!IsEmptyMemberRow(item))
                    rows.Add(item);

                rowNumber++;
            }

            return rows;
        }
        private bool IsEmptyMemberRow(ExcelMemberDto row)
        {
            return string.IsNullOrWhiteSpace(row.FirstName) &&
                   string.IsNullOrWhiteSpace(row.Phone) &&
                   string.IsNullOrWhiteSpace(row.Email);
        }

        private string NormalizePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            // Remove spaces and trim
            phone = phone.Trim().Replace(" ", "");

            if (phone.StartsWith("+"))
                phone = phone.Substring(1);

            if (phone.StartsWith("0"))
            {
                phone = "254" + phone.Substring(1);
            }
          
            else if (phone.Length == 9)
            {
                // 712345678 → +254712345678
                phone = "254" + phone;
            }
            else
            {
                // Unknown format → return null (or keep original if you prefer)
                return null;
            }

            return "+" + phone;
        }


        private string GetCellValue(IXLCell cell)
        {
            return cell?.GetValue<string>()?.Trim();
        }

        public async Task<ApiResponse<bool>> UpdateFullMembershipStatusAsync(int memberId)
        {
            var response = new ApiResponse<bool>();

            try
            {
                var result = await _membersRepository.UpdateFullMembershipStatusAsync(memberId);

                if (result == null || !result.Success || !result.Data)
                {
                    response.IsSuccess = false;
                    response.Code = "404";
                    response.Message = result?.Message ?? "Member not found or status update failed";
                    return response;
                }

                response.IsSuccess = true;
                response.Code = "200";
                response.Data = true;
                response.Message = "Membership status updated successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }
    }
}