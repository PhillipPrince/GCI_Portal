using System;
using System.Threading.Tasks;

namespace GCI_Admin.Services.IService
{
    public interface ICloudinaryService
    {
        Task<string?> UploadBase64ImageAsync(string base64String);
    }
}
