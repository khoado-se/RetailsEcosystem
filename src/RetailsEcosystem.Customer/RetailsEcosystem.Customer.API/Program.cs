using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using RetailsEcosystem.Customer.API.Options;
using RetailsEcosystem.Customer.API.Services;
using RetailsEcosystem.Customer.Infrastructure;
using RetailsEcosystem.Customer.API.Extensions;
using RetailsEcosystem.Customer.API.Middleware;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));
builder.Services.Configure<CloudinaryOptions>(
    builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton<Cloudinary>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<CloudinaryOptions>>().Value;
    if (string.IsNullOrEmpty(opts.CloudName) || string.IsNullOrEmpty(opts.ApiKey) || string.IsNullOrEmpty(opts.ApiSecret))
        throw new InvalidOperationException("Cloudinary credentials are not configured. Set Cloudinary:CloudName, ApiKey, and ApiSecret.");
    var account = new Account(opts.CloudName, opts.ApiKey, opts.ApiSecret);
    return new Cloudinary(account) { Api = { Secure = true } };
});
builder.Services.AddScoped<IFileStorageService, CloudinaryStorageService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()!)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(RetailsEcosystem.Customer.Application.Validators.RegisterDtoValidator).Assembly);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("ProductionPolicy");

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.SeedIdentityAsync();

app.Run();
