using Mapster;
using RetailsEcosystem.Customer.Domain.Entities;
using RetailsEcosystem.Customer.Shared;
using RetailsEcosystem.Customer.Shared.DTOs.Customer;
using RetailsEcosystem.Customer.Shared.DTOs.Order;

namespace RetailsEcosystem.Customer.Application.Mappings
{
    public static class MappingConfig
    {
        public static void Configure()
        {
            var config = TypeAdapterConfig.GlobalSettings;

            config.NewConfig<Category, CategoryDto>()
                .Map(dest => dest.Description, src => src.Description ?? string.Empty);

            config.NewConfig<Product, ProductDto>()
                .Map(dest => dest.ImageUrl, src => src.Images.FirstOrDefault() != null ? src.Images.First().Url : null)
                .Map(dest => dest.ImageUrls, src => src.Images.Select(i => i.Url).ToList());

            config.NewConfig<OrderItem, OrderItemDto>()
                .Map(dest => dest.ProductImageUrl,
                    src => src.Product != null && src.Product.Images.FirstOrDefault() != null
                        ? src.Product.Images.First().Url
                        : null);

            config.NewConfig<Order, OrderDto>()
                .Map(dest => dest.UserEmail, src => src.User != null ? src.User.Email ?? string.Empty : string.Empty);

            // Roles cannot be resolved from the entity — set manually after mapping
            config.NewConfig<ApplicationUser, CustomerDto>()
                .Map(dest => dest.Email, src => src.Email ?? string.Empty)
                .Ignore(dest => dest.Roles);
        }
    }
}
