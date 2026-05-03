using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Tests.Helpers.Builders;

public class CategoryBuilder
{
    private int _id = 1;
    private string _name = "Test Category";
    private string _description = "Test Description";

    public CategoryBuilder WithId(int id) { _id = id; return this; }
    public CategoryBuilder WithName(string name) { _name = name; return this; }
    public CategoryBuilder WithDescription(string description) { _description = description; return this; }

    public Category Build() => new()
    {
        Id = _id,
        Name = _name,
        Description = _description
    };
}
