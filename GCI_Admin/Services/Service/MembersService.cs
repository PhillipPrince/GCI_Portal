using ClosedXML.Excel;
using GCI_Admin.DBOperations;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class MembersService : IMembersService
    {
        private readonly MembersRepository _membersRepository;
        private readonly AppDbContext _context;
        private readonly SessionManager _sessionManager;
        private readonly Security _security = new Security();

        public MembersService(MembersRepository membersRepository, AppDbContext context, SessionManager sessionManager)
        {
            _membersRepository = membersRepository;
            _context = context;
            _sessionManager = sessionManager;
        }

        // ✅ GET ALL MEMBERS (Checks Session first)
        public async Task<ApiResponse<List<Member>>> GetAllMembersAsync()
        {
            var response = new ApiResponse<List<Member>>();

            try
            {
                // Check session cache first
                var cachedMembers = _sessionManager.GetMembersSession();
                if (cachedMembers != null && cachedMembers.Any())
                {
                    response.Data = cachedMembers;
                    response.Message = "Members retrieved from session successfully";
                    return response;
                }

                // If not in session, fetch from DB and store in session
                var result = await _membersRepository.GetAllMembersAsync();
                if (result.Success && result.Data != null)
                {
                    _sessionManager.SetMembersSession(result.Data);
                }

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

        public async Task<ApiResponse<DataTableResponse<Member>>> GetMembersDataTableAsync(int draw, int start, int length, string searchValue, int? statusId)
        {
            var response = new ApiResponse<DataTableResponse<Member>>();
            try
            {
                var result = await _membersRepository.GetMembersDataTableAsync(draw, start, length, searchValue, statusId);
                response.IsSuccess = result.Success;
                response.Message = result.Message;
                response.Data = result.Data;
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
                var failedRecords = new List<FailedMemberRecord>();

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
                            string.IsNullOrWhiteSpace(row.Phone))
                        {
                            response.FailedRecords++;
                            const string error = "Missing required fields.";
                            response.ErrorMessages.Add($"Row {row.RowNumber}: {error}");
                            failedRecords.Add(CreateFailedRecord(row, error));
                            continue;
                        }

                        // ✅ Date parsing
                        DateTime? dob = null;

                        if (!string.IsNullOrWhiteSpace(row.DateOfBirth))
                        {
                            string dateValue = row.DateOfBirth.Trim();

                            // Remove ordinal suffixes: 1st, 2nd, 3rd, 4th...
                            dateValue = Regex.Replace(
                                dateValue,
                                @"(\d+)(st|nd|rd|th)",
                                "$1",
                                RegexOptions.IgnoreCase);

                            // Year only
                            if (dateValue.Length == 4 &&
                                int.TryParse(dateValue, out int year))
                            {
                                dob = new DateTime(year, 1, 1);
                            }

                            // Excel serial date
                            else if (double.TryParse(dateValue, out double excelDate))
                            {
                                dob = DateTime.FromOADate(excelDate);
                            }

                            else
                            {
                                string[] formats =
                                {
            "dd/MM/yyyy",
            "d/M/yyyy",
            "dd-MM-yyyy",
            "d-M-yyyy",
            "dd MMMM yyyy",
            "d MMMM yyyy",
            "dd MMM yyyy",
            "d MMM yyyy",
            "yyyy-MM-dd",
            "MM/dd/yyyy",
            "M/d/yyyy"
        };

                                if (DateTime.TryParseExact(
                                        dateValue,
                                        formats,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out DateTime parsedDob))
                                {
                                    dob = parsedDob;
                                }
                                else if (DateTime.TryParse(
                                            dateValue,
                                            out parsedDob))
                                {
                                    dob = parsedDob;
                                }
                                else
                                {
                                    row.DateOfBirth = DateTime.Now.ToString();
                                    //response.FailedRecords++;

                                    //const string error = "Invalid Date of Birth format. Please verify the input. The system has temporarily set it to today's date.";
                                    //Loggers.EventLogs(error);
                                    Loggers.EventLogs($"DOB Error | Row: {row.RowNumber} | Name: {row.FirstName} {row.OtherNames} | Message: Invalid Date of Birth format. Defaulted to today.");
                                    //response.ErrorMessages.Add(
                                    //    $"Row {row.RowNumber}: {error}");

                                    //failedRecords.Add(
                                    //    CreateFailedRecord(row, error));


                                    //continue;
                                }
                            }
                        }

                        // ✅ Number of children
                        int children = 0;
                        if (!string.IsNullOrWhiteSpace(row.NumberOfChildren))
                        {
                            int.TryParse(row.NumberOfChildren, out children);
                        }

                        string phone = PhoneHelper.NormalizeKenyanPhoneOrEmail(row?.Phone ?? "");
                        string email = row?.Email?.Trim() ?? "";

                        // Skip if both are empty (avoid useless query)
                        if (string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(email))
                        {
                            response.FailedRecords++;
                            const string error = "Phone and Email cannot both be empty.";
                            response.ErrorMessages.Add($"Row {row.RowNumber}: {error}");
                            failedRecords.Add(CreateFailedRecord(row, error));
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
                            const string error = "Duplicate member.";
                            response.ErrorMessages.Add($"Row {row.RowNumber}: {error}");
                            failedRecords.Add(CreateFailedRecord(row, error));
                            continue;
                        }
                        if (string.IsNullOrEmpty(phone))
                            {
                            phone = "";

                        }
                        var gender = row.Gender?.Trim();

                        if (!string.IsNullOrEmpty(gender))
                        {
                            if (gender.Equals("M", StringComparison.OrdinalIgnoreCase))
                                gender = "Male";
                            else if (gender.Equals("F", StringComparison.OrdinalIgnoreCase))
                                gender = "Female";
                            else if (!gender.Equals("Male", StringComparison.OrdinalIgnoreCase) &&
                                     !gender.Equals("Female", StringComparison.OrdinalIgnoreCase))
                            {
                                response.FailedRecords++;
                                const string error = "Invalid Gender value. Allowed values: M, F, Male, Female.";
                                response.ErrorMessages.Add($"Row {row.RowNumber}: {error}");
                                failedRecords.Add(CreateFailedRecord(row, error));
                                continue;
                            }
                        }

                        var member = new Member
                        {
                            FirstName = row.FirstName,
                            OtherNames = row.OtherNames,
                            Phone = phone,
                            Email = row.Email,
                            Gender = gender,
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
                        response.ErrorMessages.Add(
                            $"Row {row.RowNumber}: {ex.Message}");
                        failedRecords.Add(
                            CreateFailedRecord(row, ex.Message));
                    }
                }

                if (membersToInsert.Any())
                {
                    await _context.Members.AddRangeAsync(membersToInsert);
                    await _context.SaveChangesAsync();
                }
                if (failedRecords.Any())
                {
                    var failedFileBytes =
                        GenerateFailedRecordsExcel(failedRecords);

                    response.FailedRecordsFileBase64 =
                        Convert.ToBase64String(failedFileBytes);

                    response.FailedRecordsFileName =
                        $"FailedMembers_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
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

        public async Task<ApiResponse<List<Member>>> GetMembersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _membersRepository.GetMembersByDateRangeAsync(startDate, endDate);

                return new ApiResponse<List<Member>>
                {
                    IsSuccess = response.Success,
                    Message = response.Message,
                    Data = response.Data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"MembersService -> GetMembersByDateRangeAsync -> {ex}");

                return new ApiResponse<List<Member>>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while fetching members: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<List<Member>>> GetActiveMembersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _membersRepository.GetActiveMembersByDateRangeAsync(startDate, endDate);

                return new ApiResponse<List<Member>>
                {
                    IsSuccess = response.Success,
                    Message = response.Message,
                    Data = response.Data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"MembersService -> GetActiveMembersByDateRangeAsync -> {ex}");

                return new ApiResponse<List<Member>>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while fetching active members: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<List<Member>>> GetFullMembersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var response = await _membersRepository.GetFullMembersByDateRangeAsync(startDate, endDate);

                return new ApiResponse<List<Member>>
                {
                    IsSuccess = response.Success,
                    Message = response.Message,
                    Data = response.Data
                };
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"MembersService -> GetFullMembersByDateRangeAsync -> {ex}");

                return new ApiResponse<List<Member>>
                {
                    IsSuccess = false,
                    Message = $"An error occurred while fetching full members: {ex.Message}",
                    Data = null
                };
            }
        }
        public async Task<ApiResponse<Member>> DeleteUserByPhone(string phone)
        {
            var response = new ApiResponse<Member>();
            try
            {
                var dbResponse = await _membersRepository.UpdateUserStatus(phone, 8);
                response.IsSuccess = dbResponse.Success;
                response.Message = dbResponse.Message;
                response.Code = dbResponse.Success ? "200" : "400";
                response.Data = dbResponse.Data;
                Loggers.EventLogs($"UpdateUserStatus attempt for phone {phone}: {dbResponse.Message}");
                return response;
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"UpdateUserStatus Exception for phone {phone}: {ex.Message}");
                response.IsSuccess = false;
                response.Message = "An error occurred while updating the user status.";
                response.Code = "500";
                response.Data = null;
                return response;
            }
        }

        private byte[] GenerateFailedRecordsExcel(List<FailedMemberRecord> failedRecords)
        {
            using var workbook = new XLWorkbook();

            var ws = workbook.Worksheets.Add("Failed Records");

            ws.Cell(1, 1).Value = "Row Number";
            ws.Cell(1, 2).Value = "Error";
            ws.Cell(1, 3).Value = "First Name";
            ws.Cell(1, 4).Value = "Other Names";
            ws.Cell(1, 5).Value = "Phone";
            ws.Cell(1, 6).Value = "Email";
            ws.Cell(1, 7).Value = "Gender";
            ws.Cell(1, 8).Value = "Date Of Birth";
            ws.Cell(1, 9).Value = "Marital Status";
            ws.Cell(1, 10).Value = "Spouse Name";
            ws.Cell(1, 11).Value = "Number Of Children";
            ws.Cell(1, 12).Value = "Assembly";
            ws.Cell(1, 13).Value = "Residential Address";
            ws.Cell(1, 14).Value = "Social Media Name";

            int row = 2;

            foreach (var item in failedRecords)
            {
                ws.Cell(row, 1).Value = item.RowNumber;
                ws.Cell(row, 2).Value = item.ErrorMessage;
                ws.Cell(row, 3).Value = item.FirstName;
                ws.Cell(row, 4).Value = item.OtherNames;
                ws.Cell(row, 5).Value = item.Phone;
                ws.Cell(row, 6).Value = item.Email;
                ws.Cell(row, 7).Value = item.Gender;
                ws.Cell(row, 8).Value = item.DateOfBirth;
                ws.Cell(row, 9).Value = item.MaritalStatus;
                ws.Cell(row, 10).Value = item.SpouseName;
                ws.Cell(row, 11).Value = item.NumberOfChildren;
                ws.Cell(row, 12).Value = item.Assembly;
                ws.Cell(row, 13).Value = item.ResidentialAddress;
                ws.Cell(row, 14).Value = item.SocialMediaName;

                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        private FailedMemberRecord CreateFailedRecord(
    ExcelMemberDto row,
    string errorMessage)
        {
            return new FailedMemberRecord
            {
                RowNumber = row.RowNumber,
                ErrorMessage = errorMessage,

                FirstName = row.FirstName,
                OtherNames = row.OtherNames,
                Phone = row.Phone,
                Email = row.Email,
                Gender = row.Gender,
                DateOfBirth = row.DateOfBirth,
                MaritalStatus = row.MaritalStatus,
                SpouseName = row.SpouseName,
                NumberOfChildren = row.NumberOfChildren,
                Assembly = row.Assembly,
                ResidentialAddress = row.ResidentialAddress,
                SocialMediaName = row.SocialMediaName
            };
        }
    }
}
