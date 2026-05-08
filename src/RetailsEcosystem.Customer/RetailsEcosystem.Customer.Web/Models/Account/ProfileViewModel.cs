using System.ComponentModel.DataAnnotations;

namespace RetailsEcosystem.Customer.Web.Models.Account
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        // Display-only — populated by controller GET, not submitted in form
        public string Email { get; set; } = string.Empty;

        public string? Address { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // Preserved as hidden input — no URL input shown in form.
        // Avatar file upload requires a dedicated API endpoint.
        [Url]
        public string? AvatarUrl { get; set; }
    }
}
