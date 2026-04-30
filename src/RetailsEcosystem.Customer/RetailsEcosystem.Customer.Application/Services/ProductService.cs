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

        public ProductService(
            IProductRepository productRepo,
            ICategoryRepository categoryRepo)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
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

            var productId = await _productRepo.AddProductAsync(createProduct);

            return productId;
        }

        public async Task DeleteProductAsync(int productId)
        {
            var isExist = await _productRepo.CheckExist(productId);

            if (!isExist)
            {
                throw new Exception("Product is not found!");
            }

            await _productRepo.RemoveProductAsync(productId);
        }

        public async Task<ProductDto?> FindProductByIdAsync(int productId)
        {
            var product = await _productRepo.GetProductByIdAsync(productId);

            if (product == null)
                return null;

            var categoryDto = new CategoryDto
            {
                Id = product.Category.Id,
                Name = product.Category.Name,
            };

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                CreatedDate = product.CreatedDate,
                Description = product.Description,
                UpdatedDate = product.UpdatedDate,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsFeatured = product.IsFeatured,
                SoldCount = product.SoldCount,
                Category = categoryDto,
                ImageUrl = product.Images.FirstOrDefault()?.Url,
                ImageUrls = product.Images.Select(i => i.Url).ToList()
            };
        }

        public async Task<PagedResult<ProductDto>> GetAllProductAsync(PagedRequest pagedRequest,int? categoryId)
        {
            var products = await _productRepo
                .GetAllProductAsync(pagedRequest.PageNumber, pagedRequest.PageSize, categoryId);

            var productDtos = products.Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                CreatedDate = product.CreatedDate,
                Description = product.Description,
                UpdatedDate = product.UpdatedDate,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsFeatured = product.IsFeatured,
                SoldCount = product.SoldCount,
                Category = new CategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                },
                ImageUrl = product.Images.FirstOrDefault()?.Url
            });

            var productCount = await _productRepo.GetProductCountAsync(categoryId);

            return new PagedResult<ProductDto>(
                productDtos,
                pagedRequest,
                productCount);
        }

        public async Task<PagedResult<ProductDto>> GetFeaturedProductsAsync(PagedRequest pagedRequest)
        {
            var products = await _productRepo
                .GetFeaturedProductsAsync(pagedRequest.PageNumber, pagedRequest.PageSize);

            var productDtos = products.Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                CreatedDate = product.CreatedDate,
                Description = product.Description,
                UpdatedDate = product.UpdatedDate,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsFeatured = product.IsFeatured,
                SoldCount = product.SoldCount,
                Category = new CategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                },
                ImageUrl = product.Images.FirstOrDefault()?.Url
            });

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
                throw new Exception("Product is not found!");
            }

            Category category = await _categoryRepo.GetCategoryByIdAsync(productDto.CategoryId);

            var updateProduct = new Product
            {
                Id = productDto.Id,
                Name = productDto.Name,
                CreatedDate = productDto.CreatedDate,
                Description = productDto.Description,
                UpdatedDate = productDto.UpdatedDate,
                Price = productDto.Price,
                IsFeatured = productDto.IsFeatured,
                Category = category
            };

            await _productRepo.EditProductAsync(updateProduct);
        }
    }
}
