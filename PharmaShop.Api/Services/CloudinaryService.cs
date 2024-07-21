using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using PharmaShop.Api.Abtract;

namespace PharmaShop.Api.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Crop("fill").Gravity("face").Width(500).Height(500),
                        Folder = "PharmaShop/Product"
                    };

                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }

            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return uploadResult.Url.ToString();
        }

        public async Task DeleteAsync(string imageId)
        {
            var deletionParams = new DeletionParams(imageId);
            await _cloudinary.DestroyAsync(deletionParams);
        }
    }
}
