namespace RetailsEcosystem.Customer.Web.Models
{
    public class PromoBannerViewModel
    {
        public string? Category { get; set; }
        public string Title { get; set; } = "";
        public string? Subtitle { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public string BgColor { get; set; } = "#f8f9fa";
        public string TextColor { get; set; } = "#081828";
        public string ButtonText { get; set; } = "Shop Now";
        public string? ButtonUrl { get; set; }
        public bool ReverseLayout { get; set; } = false;
        public bool IsDark { get; set; } = false;
        public string? AdditionalClass { get; set; }
    }
}
