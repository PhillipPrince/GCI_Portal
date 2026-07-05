using System.Collections.Generic;
using System.Threading.Tasks;
using GCI_Admin.Models.DTOs;
using Utils;

namespace GCI_Admin.Services.IService
{
    public interface IGalleryService
    {
        Task<ApiResponse<List<GalleryImageDto>>> GetGalleryImagesAsync();
        Task<ApiResponse<bool>> UploadGalleryImageAsync(GalleryUploadRequestDto request);
        Task<ApiResponse<bool>> DeleteGalleryImageAsync(string filename);
    }
}
