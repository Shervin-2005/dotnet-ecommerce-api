using Npgsql;
using Respawn;
using Respawn.Graph;

namespace API.IntegrationTests.Fixtures;

public class BrandGroupTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlFixture _postgresFixture = new();

    private Respawner _respawner = null!;

    public CustomWebApplicationFactory Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Start PostgreSQL
        await _postgresFixture.InitializeAsync();

        // Start the API
        Factory = new CustomWebApplicationFactory(_postgresFixture.ConnectionString);

        // Apply migrations
        await Factory.InitializeDatabaseAsync();

        // Initialize Respawn
        await using var connection = new NpgsqlConnection(_postgresFixture.ConnectionString);

        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, 
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
                TablesToIgnore =
                [
                    new Table("public", "__EFMigrationsHistory")
                ]
            });
    }

    public async Task ResetDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(_postgresFixture.ConnectionString);

        await connection.OpenAsync();

        await _respawner.ResetAsync(connection);
    }

    public async Task DisposeAsync()
    {
        if (Factory is not null)
            await Factory.DisposeAsync();

        await _postgresFixture.DisposeAsync();
    }
}