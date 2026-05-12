using Microsoft.AspNetCore.Http;
using RetailsEcosystem.Customer.API.Services;

namespace RetailsEcosystem.Customer.IntegrationTests;

internal sealed class FakeFileStorageService : IFileStorageService
{
    public Task<List<string>> SaveFilesAsync(List<IFormFile> files)
        => Task.FromResult(files.Select(f => $"fake-url/{f.FileName}").ToList());

    public Task DeleteFileAsync(string publicId)
        => Task.CompletedTask;
}
