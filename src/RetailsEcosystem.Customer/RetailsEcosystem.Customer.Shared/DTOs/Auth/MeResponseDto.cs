namespace RetailsEcosystem.Customer.Shared.DTOs.Auth
{
    /// <summary>Returned by GET /api/auth/me.</summary>
    public class MeResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
