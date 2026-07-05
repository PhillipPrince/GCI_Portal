using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Services.IService;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class GECPositionService : IGECPositionService
    {
        private readonly GECPositionRepository _repository;

        public GECPositionService(GECPositionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<List<GECPosition>>> GetAllPositionsAsync()
        {
            var response = await _repository.GetAllPositionsAsync();
            return new ApiResponse<List<GECPosition>>
            {
                IsSuccess = response.Success,
                Message = response.Message,
                Data = response.Data
            };
        }

        public async Task<ApiResponse<GECPosition>> GetPositionByIdAsync(int id)
        {
            var response = await _repository.GetPositionByIdAsync(id);
            return new ApiResponse<GECPosition>
            {
                IsSuccess = response.Success,
                Message = response.Message,
                Data = response.Data
            };
        }

        public async Task<ApiResponse<GECPosition>> CreatePositionAsync(GECPosition position)
        {
            var response = await _repository.CreatePositionAsync(position);
            return new ApiResponse<GECPosition>
            {
                IsSuccess = response.Success,
                Message = response.Message,
                Data = response.Data
            };
        }

        public async Task<ApiResponse<GECPosition>> UpdatePositionAsync(GECPosition position)
        {
            var response = await _repository.UpdatePositionAsync(position);
            return new ApiResponse<GECPosition>
            {
                IsSuccess = response.Success,
                Message = response.Message,
                Data = response.Data
            };
        }

        public async Task<ApiResponse<bool>> DeletePositionAsync(int id)
        {
            var response = await _repository.DeletePositionAsync(id);
            return new ApiResponse<bool>
            {
                IsSuccess = response.Success,
                Message = response.Message,
                Data = response.Data
            };
        }

        public async Task<ApiResponse<bool>> TogglePositionStatusAsync(int id, bool isActive)
        {
            var response = await _repository.TogglePositionStatusAsync(id, isActive);
            return new ApiResponse<bool>
            {
                IsSuccess = response.Success,
                Message = response.Message,
                Data = response.Data
            };
        }
    }
}
