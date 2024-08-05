using Microsoft.AspNetCore.Http;

namespace PharmaShop.Application.Abtract
{
    public interface ICloudinaryService
    {
        Task DeleteAsync(string imageId);
        Task<string> UploadPhotoAsync(IFormFile file);
    }
}