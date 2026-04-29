namespace RetailsEcosystem.Customer.Shared.DTOs.Customer
{
    public class CustomerDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsActive { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
