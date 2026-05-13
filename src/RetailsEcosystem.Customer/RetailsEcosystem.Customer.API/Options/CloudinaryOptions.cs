namespace RetailsEcosystem.Customer.API.Options
{
    public class CloudinaryOptions
    {
        public string CloudName { get; set; } = default!;
        public string ApiKey { get; set; } = default!;
        public string ApiSecret { get; set; } = default!;
        public string Folder { get; set; } = "products";
    }
}
