namespace RetailsEcosystem.Customer.Shared.DTOs.Customer
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
