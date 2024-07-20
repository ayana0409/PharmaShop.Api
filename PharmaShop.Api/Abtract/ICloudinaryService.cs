namespace PharmaShop.Api.Abtract
{
    public interface ICloudinaryService
    {
        Task<string> UploadPhotoAsync(IFormFile file);
    }
}