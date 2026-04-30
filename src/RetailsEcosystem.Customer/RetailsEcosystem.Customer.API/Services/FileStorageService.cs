using Microsoft.Extensions.Options;
using RetailsEcosystem.Customer.API.Options;

namespace RetailsEcosystem.Customer.API.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly FileStorageOptions _options;
        private readonly string[] _allowedExtensions = { ".jpg", ".png", ".jpeg" };

        public FileStorageService(IOptions<FileStorageOptions> options)
        {
            _options = options.Value;
        }

        public async Task<List<string>> SaveFilesAsync(List<IFormFile> files)
        {
            var result = new List<string>();

            var root = Directory.GetCurrentDirectory();
            var folder = Path.Combine(root, _options.PhysicalPath);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))
                {
                    throw new Exception("Invalid file type");
                }
            }

            foreach (var file in files)
            {
                if (file.Length <= 0) continue;

                var extension = Path.GetExtension(file.FileName).ToLower();

                var fileName = Guid.NewGuid() + extension;
                var filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var url = $"{_options.BaseUrl}/{fileName}";
                result.Add(url);
            }

            return result;
        }

        public Task DeleteFileAsync(string publicId) => Task.CompletedTask;
    }
}
