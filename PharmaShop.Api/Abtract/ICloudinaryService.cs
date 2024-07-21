namespace PharmaShop.Api.Abtract
{
    public interface ICloudinaryService
    {
        Task DeleteAsync(string imageId);
        Task<string> UploadPhotoAsync(IFormFile file);
    }
}