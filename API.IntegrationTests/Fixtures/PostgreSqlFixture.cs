using Testcontainers.PostgreSql;

namespace API.IntegrationTests.Fixtures;

public class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:17-alpine")
            .WithDatabase("neocoffee_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .Build();

    public string ConnectionString => _postgresContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }
}