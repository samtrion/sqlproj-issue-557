using Microsoft.Data.SqlClient;
using NetEvolve.Extensions.NUnit;
using System.Diagnostics.CodeAnalysis;
using System.Text;

[ExcludeFromCodeCoverage]
[IntegrationTest]
[TestFixture]
[Parallelizable]
public abstract class ContainerTestBase : ContinuousTestBase, IDisposable
{
    protected ContainerFactory ContainerFactory { get; } = new ContainerFactory();
    private bool _disposedValue;

    protected virtual string ApiVersion => "1";

    protected HttpClient Client => ContainerFactory.CreateClient();

    [OneTimeSetUp]
    public async Task InitAsync() => await ContainerFactory.InitAsync().ConfigureAwait(false);

    [OneTimeTearDown]
    public async Task CleanUpAsync() => await ContainerFactory.DisposeAsync().ConfigureAwait(false);

    [Test]
    [Order(1)]
    public Task ConnectionString_is_not_empty()
    {
        var connectionString = ContainerFactory.ConnectionString;
        Assert.That(connectionString, Is.Not.Null.Or.Empty);

        return Task.CompletedTask;
    }

    [Test]
    [Order(2)]
    public async Task Create_Database()
    {
        var result = await ContainerFactory
            .Container.ExecScriptAsync($"CREATE DATABASE [{ContainerFactory.DatabaseName}];")
            .ConfigureAwait(false);
        Assert.That(result.ExitCode, Is.EqualTo(0), result.Stderr);
    }

    [Test]
    [Order(3)]
    public async Task Connection_is_available()
    {
        using (var con = new SqlConnection(ContainerFactory.ConnectionString))
        {
            await con.OpenAsync().ConfigureAwait(false);

            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT 1;";
                var result = await cmd.ExecuteScalarAsync(default).ConfigureAwait(false);

                Assert.That(result, Is.Not.Null);
            }
        }
    }

    [Test]
    [Order(4)]
    public async Task Seed_Database()
    {
        var filePath = $"{ContainerFactory.DatabaseName}_Create.sql";
        if (!File.Exists(filePath))
        {
            filePath =
                $@".\..\..\..\..\..\src\Database.TestValues\bin\Debug\netstandard2.0\{ContainerFactory.DatabaseName}_Create.sql";
        }
        Assert.That(
            File.Exists(filePath),
            Is.True,
            $"'{ContainerFactory.DatabaseName}_Create.sql' not found."
        );

        var script = await File.ReadAllTextAsync(filePath, Encoding.UTF8).ConfigureAwait(false);
        Assert.That(script, Is.Not.Null.Or.Empty);

        var result = await ContainerFactory.Container.ExecScriptAsync(script).ConfigureAwait(false);
        Assert.That(result.ExitCode, Is.EqualTo(0), result.Stderr);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                ContainerFactory.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
