namespace RetailsEcosystem.Customer.Domain.Entities
{
    /// <summary>
    /// Persistent refresh token stored in the database.
    /// One user can have multiple active refresh tokens (multiple devices),
    /// but each logout/refresh revokes the specific token used.
    /// </summary>
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>FK to AspNetUsers.Id</summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>Cryptographically secure random 64-byte base64 string.</summary>
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ApplicationUser User { get; set; } = null!;

        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
