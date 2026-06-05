using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.IntegrationTests;

/// <summary>
/// Базовый класс для интеграционных тестов PostgreSQL с использованием Entity Framework.
/// Обеспечивает создание базы данных, её инициализацию и очистку жизненного цикла.
/// </summary>
/// <typeparam name="T">Тип DbContext</typeparam>
public abstract class NpgsqlIntegrationTestsBase<T> : IAsyncLifetime where T : DbContext
{
    private readonly NpgsqlConnectionParams _serverConnectionParams;
    private readonly Func<DbContextOptions<T>, T> _contextFactory;

    private string _dbConnectionString;
    private DbContextOptions<T> _contextOptions;

    /// <summary>
    /// Создаёт экземпляр базового класса интеграционных тестов
    /// </summary>
    /// <param name="serverConnectionParams">Параметры подключения к PostgreSQL серверу</param>
    /// <param name="contextFactory">Фабрика для создания экземпляров DbContext</param>
    public NpgsqlIntegrationTestsBase(NpgsqlConnectionParams serverConnectionParams, Func<DbContextOptions<T>, T> contextFactory)
    {
        _serverConnectionParams = serverConnectionParams
            ?? throw new ArgumentNullException(nameof(serverConnectionParams));

        _contextFactory = contextFactory
            ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        _contextOptions = await TestNpgsqlDatabaseFactory.CreateDatabaseAsync(
            _serverConnectionParams.ConnectionString, _contextFactory);

        using var context = CreateContext();
        _dbConnectionString = context.Database.GetConnectionString()!;

        await SetDataAsync(context);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await TestNpgsqlDatabaseFactory.DropDatabaseAsync(_dbConnectionString!);
    }

    /// <summary>
    /// Создаёт новый экземпляр DbContext, используя настроенные параметры
    /// </summary>
    public T CreateContext() => _contextFactory(_contextOptions);

    /// <summary>
    /// Заполняет тестовую базу начальными данными.
    /// Может быть переопределён в наследуемых тестовых классах.
    /// </summary>
    /// <param name="context">Экземпляр DbContext</param>
    public virtual Task SetDataAsync(T context) => Task.CompletedTask;
}