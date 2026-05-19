
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class MeetingsService : IMeetingsService
    {
        private readonly MeetingsRepository _meetingsRepository;
        private readonly MembersRepository _membersRepository;

        public MeetingsService(MeetingsRepository meetingsRepository)
        {
            _meetingsRepository = meetingsRepository;
        }

        public async Task<ApiResponse<List<MeetingAttendance>>> GetAllMeetingsAsync()
        {
            var response = new ApiResponse<List<MeetingAttendance>>();
            try
            {
                var result = await _meetingsRepository.GetAllMeetingsAsync();
                if (!result.Success)
                {
                    response.IsSuccess = false;
                    response.Code = "400";
                    response.Message = result.Message;
                    return response;
                }
                response.IsSuccess = true;
                response.Code = "200";
                response.Data = result.Data;
                response.Message = "Meetings retrieved successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse<MeetingFullDetails>> GetMeetingDetailsByIdAsync(int id)
        {
            var response = new ApiResponse<MeetingFullDetails>();
            MeetingFullDetails meetingFullDetails = new MeetingFullDetails();
            try
            {
                
                var meeting = await _meetingsRepository.GetMeetingDetailsByIdAsync(id);
                int recordedById = meeting.Data.RecordedBy??0;

                if (meeting.Success)
                {
                    //if(recordedById != 0)
                    //{
                    //    var user= await _membersRepository.GetMemberByIdAsync(recordedById);
                    //    meeting.Data.RecorderName = user.Success ? user.Data.FirstName + " " + user.Data.OtherNames : "Unknown";
                    //}
                    meetingFullDetails.Meeting = meeting.Data;

                }

                var financialSummary = await _meetingsRepository.GetFinancialSummaryByMeetingIdAsync(id);
                if (financialSummary.Success)
                {
                    meetingFullDetails.FinancialSummary = financialSummary.Data;
                }
                var cashBreakdowns = await _meetingsRepository.GetCashBreakdownsByMeetingIdAsync(id);
                if (cashBreakdowns.Success)
                {
                    meetingFullDetails.CashBreakdowns = cashBreakdowns.Data;
                }
                var bankCollections = await _meetingsRepository.GetBankCollectionsByMeetingIdAsync(id);
                if (bankCollections.Success)
                {
                    meetingFullDetails.BankCollections = bankCollections.Data;
                }
                var signatures = await _meetingsRepository.GetSignaturesByMeetingIdAsync(id);
                if (signatures.Success)
                {
                    //foreach(var signature in signatures.Data)
                    //{
                    //    var member = await _membersRepository.GetMemberByIdAsync(signature.SignerMemberId);
                    //    if (member.Success)
                    //    {
                    //        signature.Name= member.Data.FirstName+" "+member.Data.OtherNames;
                    //    }

                    //}
                    meetingFullDetails.Signatures = signatures.Data;
                }

                response.Data = meetingFullDetails;


                response.IsSuccess = true;
                response.Code = "200";
                response.Data = response.Data;
                response.Message = "Meeting retrieved successfully";
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