using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Tests.Helpers.Builders;

public class ProductBuilder
{
    private int _id = 1;
    private string _name = "Test Product";
    private decimal _price = 10.00m;
    private int _stock = 100;
    private int _categoryId = 1;
    private string _categoryName = "Test Category";
    private readonly List<ProductImage> _images = [];

    public ProductBuilder WithId(int id) { _id = id; return this; }
    public ProductBuilder WithName(string name) { _name = name; return this; }
    public ProductBuilder WithPrice(decimal price) { _price = price; return this; }
    public ProductBuilder WithStock(int stock) { _stock = stock; return this; }
    public ProductBuilder WithCategoryId(int categoryId) { _categoryId = categoryId; return this; }
    public ProductBuilder WithImage(string url) { _images.Add(new ProductImage { Id = _images.Count + 1, Url = url, ProductId = _id }); return this; }

    public Product Build() => new()
    {
        Id = _id,
        Name = _name,
        Price = _price,
        StockQuantity = _stock,
        CategoryId = _categoryId,
        Category = new Category { Id = _categoryId, Name = _categoryName, Description = "Test" },
        Images = [.._images],
        CreatedDate = DateTime.UtcNow,
        UpdatedDate = DateTime.UtcNow
    };
}
