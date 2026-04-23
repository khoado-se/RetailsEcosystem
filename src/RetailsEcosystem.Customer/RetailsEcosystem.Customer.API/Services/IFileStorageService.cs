namespace RetailsEcosystem.Customer.API.Services
{
    public interface IFileStorageService
    {
        Task<List<string>> SaveFilesAsync(List<IFormFile> files);
    }
}
