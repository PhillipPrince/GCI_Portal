using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.DBOperations.Repositories;
using GCI_Admin.Models;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Services.IService;
using GCI_Admin.Utils;
using Utils;
using GCI_Admin.DBOperations;

namespace GCI_Admin.Services.Service
{
    public class GalleryService : IGalleryService
    {
        private readonly GalleryRepository _galleryRepository;
        private readonly SystemConfigRepository _systemConfigRepository;
        private readonly SessionManager _sessionManager;
        private readonly AppDbContext _context;

        public GalleryService(GalleryRepository galleryRepository, SystemConfigRepository systemConfigRepository, SessionManager sessionManager, AppDbContext context)
        {
            _galleryRepository = galleryRepository;
            _systemConfigRepository = systemConfigRepository;
            _context = context;
            _sessionManager = sessionManager;
        }

        private async Task<string> GetImageBasePathAsync()
        {
            var user = _sessionManager.GetUserSession<Member>();
            var imageBasePath = await SystemConfigHelper.GetImageBasePathAsync(_systemConfigRepository);
            int ? assemblyId = int.Parse(user?.Assembly);
            var assemblyName=_context.Assemblies.FirstOrDefault(a => a.Id == assemblyId).Name;

            if (user != null && user.UserRole == 2 && !string.IsNullOrEmpty(user.Assembly))
            {
                imageBasePath = Path.Combine(imageBasePath, "Assemblies", assemblyName);
            }
            
            return imageBasePath;
        }

        public async Task<ApiResponse<List<GalleryImageDto>>> GetGalleryImagesAsync()
        {
            var imageBasePath = await GetImageBasePathAsync();
            var dbResponse = await _galleryRepository.GetGalleryImagesAsync(imageBasePath);

            return new ApiResponse<List<GalleryImageDto>>
            {
                IsSuccess = dbResponse.Success,
                Data = dbResponse.Data,
                Message = dbResponse.Message
            };
        }

        public async Task<ApiResponse<bool>> UploadGalleryImageAsync(GalleryUploadRequestDto request)
        {
            if (string.IsNullOrEmpty(request?.ImageBase64))
                return new ApiResponse<bool> { IsSuccess = false, Message = "No image data provided." };

            var imageBasePath = await GetImageBasePathAsync();
            if (string.IsNullOrEmpty(imageBasePath))
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Image base path not configured." };
            }

            var dbResponse = await _galleryRepository.UploadGalleryImageAsync(request, imageBasePath);

            return new ApiResponse<bool>
            {
                IsSuccess = dbResponse.Success,
                Data = dbResponse.Data,
                Message = dbResponse.Message
            };
        }

        public async Task<ApiResponse<bool>> DeleteGalleryImageAsync(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename) || !filename.StartsWith("gallery_"))
                return new ApiResponse<bool> { IsSuccess = false, Message = "Invalid gallery filename." };

            var imageBasePath = await GetImageBasePathAsync();
            if (string.IsNullOrEmpty(imageBasePath))
            {
                return new ApiResponse<bool> { IsSuccess = false, Message = "Image base path not configured." };
            }

            var dbResponse = await _galleryRepository.DeleteGalleryImageAsync(filename, imageBasePath);

            return new ApiResponse<bool>
            {
                IsSuccess = dbResponse.Success,
                Data = dbResponse.Data,
                Message = dbResponse.Message
            };
        }
    }
}
