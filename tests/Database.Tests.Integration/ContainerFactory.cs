using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Diagnostics.CodeAnalysis;
using Testcontainers.MsSql;

[ExcludeFromCodeCoverage]
public sealed class ContainerFactory : WebApplicationFactory<Program>
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithPassword("ourStrong!123")
        .Build();

    public const string DatabaseName = "foo";

    public string ConnectionString =>
        _container
            .GetConnectionString()
            .Replace("master", DatabaseName, StringComparison.OrdinalIgnoreCase);

    public MsSqlContainer Container => _container;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var testConfigurations = GetTestConfiguration();

        _ = builder.ConfigureHostConfiguration(config =>
        {
            _ = config.AddInMemoryCollection(testConfigurations);
        });

        return base.CreateHost(builder);
    }

    public async Task InitAsync() => await _container.StartAsync().ConfigureAwait(false);

    public override async ValueTask DisposeAsync()
    {
        await _container.StopAsync().ConfigureAwait(false);
        await _container.DisposeAsync().ConfigureAwait(false);
        await base.DisposeAsync().ConfigureAwait(false);
    }

    private Dictionary<string, string?> GetTestConfiguration() =>
        new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            { "ConnectionStrings:DbConnection", ConnectionString },
            { "DisableHealthChecks", bool.TrueString },
        };
}