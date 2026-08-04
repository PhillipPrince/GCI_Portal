using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class TitlePrefixService : ITitlePrefixService
    {
        private readonly TitlePrefixRepository _repository;

        public TitlePrefixService(TitlePrefixRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<List<TitlePrefix>>> GetAllPrefixesAsync()
        {
            var result = await _repository.GetAllPrefixesAsync();
            return new ApiResponse<List<TitlePrefix>>
            {
                IsSuccess = result.Success,
                Message = result.Message,
                Data = result.Data
            };
        }

        public async Task<ApiResponse<List<TitlePrefix>>> GetActivePrefixesAsync()
        {
            var result = await _repository.GetActivePrefixesAsync();
            return new ApiResponse<List<TitlePrefix>>
            {
                IsSuccess = result.Success,
                Message = result.Message,
                Data = result.Data
            };
        }

        public async Task<ApiResponse<TitlePrefix>> GetPrefixByIdAsync(int id)
        {
            var result = await _repository.GetPrefixByIdAsync(id);
            return new ApiResponse<TitlePrefix>
            {
                IsSuccess = result.Success,
                Message = result.Message,
                Data = result.Data
            };
        }

        public async Task<ApiResponse<TitlePrefix>> CreatePrefixAsync(TitlePrefixDto dto)
        {
            var result = await _repository.CreatePrefixAsync(dto);
            return new ApiResponse<TitlePrefix>
            {
                IsSuccess = result.Success,
                Message = result.Message,
                Data = result.Data
            };
        }

        public async Task<ApiResponse<TitlePrefix>> UpdatePrefixAsync(TitlePrefixDto dto)
        {
            var result = await _repository.UpdatePrefixAsync(dto);
            return new ApiResponse<TitlePrefix>
            {
                IsSuccess = result.Success,
                Message = result.Message,
                Data = result.Data
            };
        }

        public async Task<ApiResponse<bool>> DeletePrefixAsync(int id)
        {
            var result = await _repository.DeletePrefixAsync(id);
            return new ApiResponse<bool>
            {
                IsSuccess = result.Success,
                Message = result.Message,
                Data = result.Data
            };
        }

        public async Task<ApiResponse<bool>> ToggleStatusAsync(int id, bool isActive)
        {
            var result = await _repository.ToggleStatusAsync(id, isActive);
            return new ApiResponse<bool>
            {
                IsSuccess = result.Success,
                Message = result.Message,
                Data = result.Data
            };
        }
    }
}
