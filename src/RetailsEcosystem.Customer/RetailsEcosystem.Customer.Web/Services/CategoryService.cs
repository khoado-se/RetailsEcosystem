using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Web.Interfaces;
using System.Text;

namespace RetailsEcosystem.Customer.Web.Services
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly string CategoryUrl = "api/Categories";
        public CategoryService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }
        public async Task<PagedResult<CategoryDto>> GetAllAsync(PagedRequest? pagedRequest)
        {
            StringBuilder url = new StringBuilder(CategoryUrl);
            if (pagedRequest != null) {
                url.Append($"?pageNumber={pagedRequest.PageNumber}&pageSize={pagedRequest.PageSize}");
            }

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{CategoryUrl.ToString()}"
            );
            return await SendAsync<PagedResult<CategoryDto>>(request);
        }
    }
}
