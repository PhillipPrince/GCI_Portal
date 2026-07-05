using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using GCI_Admin.Services.IService;
using GCI_Admin.Models;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace GCI_Admin.Services.Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinaryConfig _config;

        public CloudinaryService(IOptions<CloudinaryConfig> config)
        {
            _config = config.Value;

            var cloudName = _config.CloudName ?? "demo";
            var apiKey = _config.ApiKey ?? "api_key";
            var apiSecret = _config.ApiSecret ?? "api_secret";

            Account account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string?> UploadBase64ImageAsync(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return null;

            try
            {
                // Remove data:image/...;base64, prefix if present
                var base64Data = Regex.Replace(base64String, @"^data:image\/[a-zA-Z]+;base64,", string.Empty);
                
                byte[] bytes = Convert.FromBase64String(base64Data);
                
                using var stream = new MemoryStream(bytes);
                
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), stream),
                    Folder = "gci_profiles"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                
                if (uploadResult.Error != null)
                {
                    Loggers.DoLogs($"Cloudinary Upload Error: {uploadResult.Error.Message}");
                    return null;
                }

                return uploadResult.SecureUrl?.ToString();
            }
            catch (Exception ex)
            {
                Loggers.DoLogs($"Cloudinary Upload Exception: {ex.Message}");
                return null;
            }
        }
    }
}
