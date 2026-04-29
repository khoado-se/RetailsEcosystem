using Microsoft.AspNetCore.Identity;

namespace RetailsEcosystem.Customer.Domain.Entities
{
    /// <summary>
    /// Extends IdentityUser with domain-specific profile fields.
    /// Kept in Domain so other layers can reference the type without
    /// pulling in Infrastructure or Application concerns.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Soft-disable an account without deleting it.
        /// AuthService checks this flag before issuing tokens.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
