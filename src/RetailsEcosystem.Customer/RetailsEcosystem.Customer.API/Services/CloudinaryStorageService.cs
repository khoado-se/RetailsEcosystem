using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace RetailsEcosystem.Customer.API.Services
{
    public class CloudinaryStorageService : IFileStorageService
    {
        private readonly Cloudinary _cloudinary;
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        public CloudinaryStorageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<List<string>> SaveFilesAsync(List<IFormFile> files)
        {
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(ext))
                    throw new InvalidOperationException($"File type '{ext}' is not allowed. Allowed: {string.Join(", ", AllowedExtensions)}");
            }

            var urls = new List<string>();
            foreach (var file in files)
            {
                if (file.Length <= 0) continue;

                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "products",
                    UniqueFilename = true,
                    Overwrite = false
                };

                var result = await _cloudinary.UploadAsync(uploadParams);
                if (result.Error != null)
                    throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

                urls.Add(result.SecureUrl.ToString());
            }

            return urls;
        }

        public async Task DeleteFileAsync(string publicId)
        {
            var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
            if (result.Error != null)
                throw new InvalidOperationException($"Cloudinary delete failed: {result.Error.Message}");
        }
    }
}
