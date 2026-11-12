using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using API.IntegrationTests.Authentication;
using API.IntegrationTests.Fixtures;
using Application.DTOs.Category;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.IntegrationTests.Controllers;

[Collection("IntegrationTests")]
public class CategoryControllerTests : IAsyncLifetime
{
    private readonly IntegrationTestFixture _fixture;
    private readonly HttpClient _client;

    public CategoryControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Factory.CreateClient();
    }

    public Task InitializeAsync()
    {
        return _fixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetTree_WhenNoCategoriesExist_ReturnsOkWithEmptyCollection()
    {
        // Act
        var response = await _client.GetAsync("/api/Category/GetTree/tree");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.Content.ReadFromJsonAsync<List<CategoryTreeDto>>();

        Assert.NotNull(categories);
        Assert.Empty(categories);
    }

    [Fact]
    public async Task GetTree_WhenCategoriesHaveParentChildRelationships_ReturnsCorrectTree()
    {
        // Arrange
        var coffee = new Category
        {
            CategoryName = "Coffee",
            MainImageUrl = "coffee.jpg",
            ImageFolderId = Guid.NewGuid()
        };

        var espresso = new Category
        {
            CategoryName = "Espresso",
            MainImageUrl = "espresso.jpg",
            ImageFolderId = Guid.NewGuid(),
            ParentCategory = coffee
        };

        var arabica = new Category
        {
            CategoryName = "Arabica Espresso",
            MainImageUrl = "arabica.jpg",
            ImageFolderId = Guid.NewGuid(),
            ParentCategory = espresso
        };

        await using (var scope = _fixture.Factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Categories.Add(coffee);
            dbContext.Categories.Add(espresso);
            dbContext.Categories.Add(arabica);

            await dbContext.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/Category/GetTree/tree");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.Content.ReadFromJsonAsync<List<CategoryTreeDto>>();

        Assert.NotNull(categories);
        Assert.Single(categories);

        var root = categories[0];

        Assert.Equal("Coffee", root.CategoryName);
        Assert.Single(root.SubCategories);

        var child = root.SubCategories[0];

        Assert.Equal("Espresso", child.CategoryName);
        Assert.Single(child.SubCategories);

        Assert.Equal("Arabica Espresso", child.SubCategories[0].CategoryName);
    }
    
    [Fact]
    public async Task GetAll_WhenNoCategoriesExist_ReturnsOkWithEmptyCollection()
    {
        // Act
        var response = await _client.GetAsync("/api/Category/GetAll");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();

        Assert.NotNull(categories);
        Assert.Empty(categories);
    }
    
    [Fact]
    public async Task GetAll_WhenCategoriesExist_ReturnsOkWithCategories()
    {
        // Arrange
        var coffee = new Category
        {
            CategoryName = "Coffee",
            MainImageUrl = "coffee.jpg",
            ImageFolderId = Guid.NewGuid()
        };

        var espresso = new Category
        {
            CategoryName = "Espresso",
            MainImageUrl = "espresso.jpg",
            ImageFolderId = Guid.NewGuid(),
            ParentCategory = coffee
        };

        await using (var scope = _fixture.Factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Categories.AddRange(coffee, espresso);

            await dbContext.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/Category/GetAll");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();

        Assert.NotNull(categories);
        Assert.Equal(2, categories.Count);

        var coffeeDto = categories.Single(c => c.CategoryName == "Coffee");
        var espressoDto = categories.Single(c => c.CategoryName == "Espresso");

        Assert.Null(coffeeDto.ParentCategoryId);
        Assert.Equal(coffee.CategoryId, espressoDto.ParentCategoryId);
    }

    [Fact]
    public async Task Create_AsAdminWithValidData_ReturnsCreated()
    {
        // Arrange
        var client = CreateAuthenticatedClient("Admin");
        
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("New Integration Category"), "CategoryName");
        
        // Simulate a file upload for the [FromForm] binding
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3, 4 });
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(fileContent, "File", "test-image.jpg");

        // Act
        var response = await client.PostAsync("/api/Category/Create", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var createdCategory = await response.Content.ReadFromJsonAsync<CategoryDto>();
        Assert.NotNull(createdCategory);
        Assert.Equal("New Integration Category", createdCategory.CategoryName);
    }

    [Fact]
    public async Task Create_AsNonAdmin_ReturnsForbidden()
    {
        // Arrange
        var client = CreateAuthenticatedClient("Customer");
        
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("Should Fail"), "CategoryName");

        // Act
        var response = await client.PostAsync("/api/Category/Create", content);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_AsAdmin_ReturnsNoContent()
    {
        // Arrange
        var existingCategory = new Category
        {
            CategoryName = "Old Category Name",
            MainImageUrl = "old.jpg",
            ImageFolderId = Guid.NewGuid()
        };

        // Seed the database
        await using (var scope = _fixture.Factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Categories.Add(existingCategory);
            await dbContext.SaveChangesAsync();
        }

        var client = CreateAuthenticatedClient("Admin");
        
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("Updated Category Name"), "CategoryName");
        
        var fileContent = new ByteArrayContent(new byte[] { 5, 6, 7, 8 });
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(fileContent, "File", "new-image.jpg");

        // Act
        var response = await client.PutAsync($"/api/Category/Update/{existingCategory.CategoryId}", content);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AsAdmin_ReturnsNoContent()
    {
        // Arrange
        var categoryToDelete = new Category
        {
            CategoryName = "To Be Deleted",
            MainImageUrl = "delete-me.jpg",
            ImageFolderId = Guid.NewGuid()
        };
        
        await using (var scope = _fixture.Factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Categories.Add(categoryToDelete);
            await dbContext.SaveChangesAsync();
        }

        var client = CreateAuthenticatedClient("Admin");

        // Act
        var response = await client.DeleteAsync($"/api/Category/Delete/{categoryToDelete.CategoryId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        await using (var verifyScope = _fixture.Factory.Services.CreateAsyncScope())
        {
            var dbContext = verifyScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var exists = await dbContext.Categories.AnyAsync(c => c.CategoryId == categoryToDelete.CategoryId);
            Assert.False(exists);
        }
    }
    
    private HttpClient CreateAuthenticatedClient(string role)
    {
        var client = _fixture.Factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                            options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
                });
            })
            .CreateClient();

        client.DefaultRequestHeaders.Add("X-Test-Role", role);

        return client;
    }
}