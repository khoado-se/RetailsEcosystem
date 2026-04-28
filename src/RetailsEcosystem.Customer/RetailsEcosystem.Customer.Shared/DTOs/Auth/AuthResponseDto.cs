namespace RetailsEcosystem.Customer.Shared.DTOs.Auth
{
    /// <summary>
    /// Returned on successful login or refresh.
    /// The refresh token is NOT included here — it is sent via httpOnly cookie.
    /// </summary>
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto User { get; set; } = null!;
    }

    public class UserInfoDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
