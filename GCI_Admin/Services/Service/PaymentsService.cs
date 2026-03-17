using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Repo_GCI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class PaymentsService : IPaymentsService
    {
        private readonly PaymentsRepository _repo;

        public PaymentsService(PaymentsRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<Payment>>> GetAllAsync()
        {
            var response = new ApiResponse<List<Payment>>();

            try
            {
                var result = await _repo.GetAllAsync();

                response.IsSuccess = result.Success;
                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = result.Success ? "200" : "400";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<List<Payment>>> GetByMemberIdAsync(int memberId)
        {
            var response = new ApiResponse<List<Payment>>();

            try
            {
                var result = await _repo.GetByMemberIdAsync(memberId);

                response.IsSuccess = result.Success;
                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = result.Success ? "200" : "400";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Code = "500";
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ApiResponse<List<AccountReferenceSummaryDto>>> GetAccountReferenceSummaryAsync()
        {
            var response = new ApiResponse<List<AccountReferenceSummaryDto>>();

            try
            {
                var result = await _repo.GetAccountReferenceSummaryAsync();

                response.IsSuccess = result.Success;
                response.Data = result.Data;
                response.Message = result.Message;
                response.Code = result.Success ? "200" : "400";
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