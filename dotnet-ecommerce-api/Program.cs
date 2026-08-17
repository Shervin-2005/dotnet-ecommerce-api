using Amazon.S3;
using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Infrastructure.Services;
using Infrastructure.Settings;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    Environment.GetEnvironmentVariable("DOTNET_ECOMMERCE_API")
 ?? throw new Exception("Environment variable DOTNET_ECOMMERCE_API is not set");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IImageStorageService, S3ImageStorageService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = builder.Configuration["AutoMapper:ServiceApiKey"];
}, typeof(MappingProfile));

builder.Services.Configure<S3Settings>(
    builder.Configuration.GetSection("ArvanStorage")
);

builder.Services.Configure<SmsSettings>(
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

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings are not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
               Reference = new OpenApiReference
               {
                   Type = ReferenceType.SecurityScheme,
                   Id = "Bearer"
               }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(options => 
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
