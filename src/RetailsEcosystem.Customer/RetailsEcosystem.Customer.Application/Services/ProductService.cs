using Mapster;
using RetailsEcosystem.Customer.Application.Exceptions;
using RetailsEcosystem.Customer.Application.Interfaces;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Domain.Interface;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs;
using RetailsEcosystem.Customer.Shared.DTOs.Product;

namespace RetailsEcosystem.Customer.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IProductRepository productRepo,
            ICategoryRepository categoryRepo,
            IUnitOfWork unitOfWork)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateProductAsync(CreateProductDto productDto)
        {
            Category category = await _categoryRepo.GetCategoryByIdAsync(productDto.CategoryId);

            var createProduct = new Product
            {
                Name = productDto.Name,
                CreatedDate = productDto.CreatedDate,
                Description = productDto.Description,
                UpdatedDate = productDto.UpdatedDate,
                Price = productDto.Price,
                IsFeatured = productDto.IsFeatured,
                Category = category
            };

            await _productRepo.AddProductAsync(createProduct);
            await _unitOfWork.SaveChangesAsync();

            return createProduct.Id;
        }

        public async Task DeleteProductAsync(int productId)
        {
            var isExist = await _productRepo.CheckExist(productId);

            if (!isExist)
            {
                throw new NotFoundException("Product is not found!");
            }

            await _productRepo.RemoveProductAsync(productId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ProductDto?> FindProductByIdAsync(int productId)
        {
            var product = await _productRepo.GetProductByIdAsync(productId);
            return product?.Adapt<ProductDto>();
        }

        public async Task<PagedResult<ProductDto>> GetAllProductAsync(PagedRequest pagedRequest, int? categoryId)
        {
            var products = await _productRepo
                .GetAllProductAsync(pagedRequest.PageNumber, pagedRequest.PageSize, categoryId, pagedRequest.Search,
                    pagedRequest.IsFeatured, pagedRequest.SortBy, pagedRequest.SortDesc);

            var productDtos = products.Select(p => p.Adapt<ProductDto>());

            var productCount = await _productRepo.GetProductCountAsync(categoryId, search: pagedRequest.Search, isFeatured: pagedRequest.IsFeatured);

            return new PagedResult<ProductDto>(
                productDtos,
                pagedRequest,
                productCount);
        }

        public async Task<PagedResult<ProductDto>> GetFeaturedProductsAsync(PagedRequest pagedRequest)
        {
            var products = await _productRepo
                .GetFeaturedProductsAsync(pagedRequest.PageNumber, pagedRequest.PageSize);

            var productDtos = products.Select(p => p.Adapt<ProductDto>());

            var productCount = await _productRepo.GetProductCountAsync(isFeature: true);

            return new PagedResult<ProductDto>(
                productDtos,
                pagedRequest,
                productCount);
        }

        public async Task UpdateProductAsync(UpdateProductDto productDto)
        {
            var isExist = await _productRepo.CheckExist(productDto.Id);

            if (!isExist)
            {
                throw new NotFoundException("Product is not found!");
            }

            var category = await _categoryRepo.GetCategoryByIdAsync(productDto.CategoryId)
                ?? throw new NotFoundException($"Category {productDto.CategoryId} not found.");

            var updateProduct = new Product
            {
                Id = productDto.Id,
                Name = productDto.Name,
                CreatedDate = productDto.CreatedDate,
                Description = productDto.Description,
                UpdatedDate = productDto.UpdatedDate,
                Price = productDto.Price,
                IsFeatured = productDto.IsFeatured,
                CategoryId = category.Id,
                Category = category
            };

            await _productRepo.EditProductAsync(updateProduct);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
