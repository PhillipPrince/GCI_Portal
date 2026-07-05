using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GCI_Admin.Models.DTOs;
using GCI_Admin.Utils;
using Utils;

namespace GCI_Admin.DBOperations.Repositories
{
    public class GalleryRepository
    {
        public async Task<DbResponse<List<GalleryImageDto>>> GetGalleryImagesAsync(string imageBasePath)
        {
            var images = new List<GalleryImageDto>();
            try
            {
                if (!string.IsNullOrEmpty(imageBasePath) && Directory.Exists(imageBasePath))
                {
                    var files = Directory.GetFiles(imageBasePath, "gallery_*.*")
                                         .OrderByDescending(f => new FileInfo(f).CreationTime)
                                         .ToList();

                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        var imageBytes = ImageHelper.ReadImage(imageBasePath, fileName);
                        
                        if (imageBytes != null)
                        {
                            images.Add(new GalleryImageDto
                            {
                                FileName = Path.GetFileName(file),
                                ImageBytes = Convert.ToBase64String(imageBytes),
                                SizeBytes = new FileInfo(file).Length,
                                CreatedAt = new FileInfo(file).CreationTime
                            });
                        }
                    }
                }
                return new DbResponse<List<GalleryImageDto>> { Success = true, Data = images };
            }
            catch (Exception ex)
            {
                return new DbResponse<List<GalleryImageDto>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<DbResponse<bool>> UploadGalleryImageAsync(GalleryUploadRequestDto request, string imageBasePath)
        {
            try
            {
                if (!Directory.Exists(imageBasePath))
                    Directory.CreateDirectory(imageBasePath);

                var imageBytes = ImageHelper.RemoveBase64Prefix(request.ImageBase64);
                var filename = $"gallery_{Guid.NewGuid().ToString("N")}";
                
                var savedPath = ImageHelper.SaveImage(imageBytes, imageBasePath, filename, "jpg");

                if (savedPath != null)
                {
                    return new DbResponse<bool> { Success = true, Data = true, Message = "Image uploaded successfully." };
                }
                
                return new DbResponse<bool> { Success = false, Data = false, Message = "Failed to save image to configured path." };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Message = ex.Message };
            }
        }

        public async Task<DbResponse<bool>> DeleteGalleryImageAsync(string filename, string imageBasePath)
        {
            try
            {
                var safeName = Path.GetFileName(filename);
                var fullPath = Path.Combine(imageBasePath, safeName);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return new DbResponse<bool> { Success = true, Data = true, Message = "Image deleted." };
                }
                
                return new DbResponse<bool> { Success = false, Data = false, Message = "Image not found." };
            }
            catch (Exception ex)
            {
                return new DbResponse<bool> { Success = false, Message = ex.Message };
            }
        }
    }
}
