using System.ComponentModel.DataAnnotations;

namespace RetailsEcosystem.Customer.Web.Models.Account
{
    public class ProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Avatar URL")]
        [Url]
        public string? AvatarUrl { get; set; }

        public string? Address { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}
