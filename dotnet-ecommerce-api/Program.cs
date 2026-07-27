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

builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODE2NTYwMDAwIiwiaWF0IjoiMTc4NTA3MTI2NiIsImFjY291bnRfaWQiOiIwMTlmOWU4Nzg3YjI3YTk3OGY3ZDU2OGU2MjY3YjM5OSIsImN1c3RvbWVyX2lkIjoiMDE5ZjllODc4N2IyN2E5NzhmN2Q1NjhlNjI2N2IzOTkiLCJzdWJfaWQiOiItIiwiZWRpdGlvbiI6IjAiLCJ0eXBlIjoiMiJ9.FQe68VIkVfMM91Oyw2TAcHGHh3mMdlbk6nSh_BRIoXYTLsCKgHB24_51_KnUPLU7cdwkkJplu8k5qQkgNPRqu6oXoYJ4d_lL-_j2K41fmCG_aMWKcXfuQ7hDYqEFD1xGNhEV7EQVrCiBr7DBxN93jfkyq8q7-IxaBUGNe8MHtHWaMDcVBiIVqlXkPCB2SeqkQmrtClwveKe4Sivixe91bB6YHNAxRNg3j3Q0iTF-KgueXSxdmDpsWvmrts_xMMkbZ1Mzr4LoN6HCBtXhMwi-_jmMmXi4baLLyk3kndYm_6nDJ-_UvOWYCb8GkLrp1yg0WpJbaNp7ZnqIX4WwcIWHbw";
}, typeof(MappingProfile));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
    Console.WriteLine($"ServiceUrl: '{settings.ServiceUrl}'");
    Console.WriteLine($"AccessKey: '{settings.AccessKey}'");
    Console.WriteLine($"BucketName: '{settings.BucketName}'");
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
