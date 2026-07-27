using Amazon.S3;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Infastructure.Services;
using Infastructure.Settings;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString =
    Environment.GetEnvironmentVariable("DOTNET_ECOMMERCE_API")
 ?? throw new Exception("Environment variable DOTNET_ECOMMERCE_API is not set");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IImageStorageService, S3ImageStorageService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = builder.Configuration["AutoMapper:ServiceApiKey"];
}, typeof(MappingProfile));

builder.Services.Configure<S3Settings>(
    builder.Configuration.GetSection("ArvanStorage")
);

builder.Services.Configure<S3Settings>(
    builder.Configuration.GetSection("OTPOptions")
);

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var settings = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<S3Settings>>().Value;
    var credentials = new Amazon.Runtime.BasicAWSCredentials(
        settings.AccessKey,
        settings.SecretKey);

    var config = new AmazonS3Config
    {
        ServiceURL = settings.ServiceUrl,
        ForcePathStyle = true
    };

    return new AmazonS3Client(credentials, config);
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
