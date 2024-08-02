using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using PharmaShop.Api.Abtract;
using System.Text.RegularExpressions;

namespace PharmaShop.Api.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _folder = "PharmaShop/Product";

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
                        Folder = _folder
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

        public async Task DeleteAsync(string imageUrl)
        {
            string pattern = _folder + @"/([^/]+)\.png";
            Match match = Regex.Match(imageUrl, pattern);

            if (match.Success)
            {
                string publicId = _folder + "/" + match.Groups[1].Value;
                var deletionParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deletionParams);

                if (result.Result != "ok")
                {
                    var errorMessage = result.Error?.Message ?? "Unknown error occurred.";
                    throw new Exception($"Failed to delete image: {errorMessage}");
                }
            }
            else
            {
                throw new Exception("No match found.");
            }
        }

    }
}
