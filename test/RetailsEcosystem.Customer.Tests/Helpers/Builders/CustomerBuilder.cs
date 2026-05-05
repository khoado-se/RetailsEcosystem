using RetailsEcosystem.Customer.Domain.Entities;

namespace RetailsEcosystem.Customer.Tests.Helpers.Builders;

public class CustomerBuilder
{
    private string _id = "user-123";
    private string _email = "test@example.com";
    private string _fullName = "Test User";
    private bool _isActive = true;

    public CustomerBuilder WithId(string id) { _id = id; return this; }
    public CustomerBuilder WithEmail(string email) { _email = email; return this; }
    public CustomerBuilder WithFullName(string fullName) { _fullName = fullName; return this; }
    public CustomerBuilder WithIsActive(bool isActive) { _isActive = isActive; return this; }

    public ApplicationUser Build() => new()
    {
        Id = _id,
        Email = _email,
        UserName = _email,
        FullName = _fullName,
        IsActive = _isActive
    };
}
