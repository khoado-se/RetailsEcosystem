using Microsoft.AspNetCore.Authentication.Cookies;
using RetailsEcosystem.Customer.Shared.Settings;
using RetailsEcosystem.Customer.Web.Attributes;
using RetailsEcosystem.Customer.Web.Interfaces;
using RetailsEcosystem.Customer.Web.Options;
using RetailsEcosystem.Customer.Web.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>()!;

var culture = new CultureInfo(builder.Configuration.GetValue<string>("Currency:CultureInfo")!);

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    // Sign out + redirect to login when the stored JWT is expired (pre-action check)
    options.Filters.Add<TokenExpiryFilter>();
    // Sign out + redirect to login when the API returns 401 (handles timing edge cases)
    options.Filters.Add<ApiUnauthorizedFilter>();
});
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddHttpClient("MyApi", client =>
{
    client.BaseAddress = new Uri(apiSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login";
        options.LogoutPath = "/account/logout";
        options.AccessDeniedPath = "/account/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.Configure<VnpaySettings>(builder.Configuration.GetSection("VnpaySettings"));
builder.Services.AddScoped<IVnpayWebService, VnpayWebService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
