using System.Net;
using System.Net.Http.Json;
using API.IntegrationTests.Fixtures;
using Application.DTOs.Brand;

namespace API.IntegrationTests.Controllers;

[Collection("BrandGroup")]
public class BrandGroupControllerTests : IAsyncLifetime
{
    private readonly BrandGroupTestFixture _fixture;
    private readonly HttpClient _client;

    public BrandGroupControllerTests(BrandGroupTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Factory.CreateClient();
    }

    public Task InitializeAsync()
    {
        return _fixture.ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await Task.CompletedTask;
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/api/BrandGroup/GetAll");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var groups = await response.Content
            .ReadFromJsonAsync<List<BrandGroupDto>>();

        Assert.NotNull(groups);
    }
    
    [Fact]
    public async Task GetById_WhenBrandGroupDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        const int nonExistingId = 999999;

        // Act
        var response = await _client.GetAsync(
            $"/api/BrandGroup/GetById/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}