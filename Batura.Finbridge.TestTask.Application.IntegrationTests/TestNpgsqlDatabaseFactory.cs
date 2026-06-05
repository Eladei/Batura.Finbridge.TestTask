using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Batura.Finbridge.TestTask.Application.IntegrationTests;

/// <summary>
/// Фабрика для создания и управления тестовыми базами данных PostgreSQL.
/// Отвечает за создание базы данных, выполнение миграций и очистку.
/// </summary>
public static class TestNpgsqlDatabaseFactory
{
    /// <summary>
    /// Создаёт новую изолированную базу данных PostgreSQL для интеграционных тестов
    /// и применяет миграции EF Core.
    /// </summary>
    /// <typeparam name="TContext">Тип DbContext</typeparam>
    /// <param name="connectionString">Базовая строка подключения к PostgreSQL</param>
    /// <param name="contextFactory">Фабрика для создания экземпляров DbContext</param>
    /// <returns>Настроенные DbContextOptions для созданной тестовой базы данных</returns>
    public static async Task<DbContextOptions<TContext>> CreateDatabaseAsync<TContext>(
        string connectionString,
        Func<DbContextOptions<TContext>, TContext> contextFactory)
        where TContext : DbContext
    {
        var dbName = "test_db_" + Guid.NewGuid().ToString("N");

        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = dbName
        };

        var masterConnectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        }.ToString();

        using (var connection = new NpgsqlConnection(masterConnectionString))
        {
            await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE \"{dbName}\"";

            await cmd.ExecuteNonQueryAsync();
        }

        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(builder.ToString())
            .Options;

        using (var context = contextFactory(options))
        {
            await context.Database.MigrateAsync();
        }

        return options;
    }

    /// <summary>
    /// Удаляет ранее созданную тестовую базу данных PostgreSQL.
    /// Завершает активные подключения перед удалением.
    /// </summary>
    /// <param name="connectionString">Строка подключения к целевой базе данных</param>
    public static async Task DropDatabaseAsync(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var dbName = builder.Database;

        var masterConnectionString = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres"
        }.ToString();

        using var connection = new NpgsqlConnection(masterConnectionString);
        await connection.OpenAsync();

        // Завершение активных подключений к базе данных
        using (var terminateCmd = connection.CreateCommand())
        {
            terminateCmd.CommandText = $@"
                SELECT pg_terminate_backend(pid)
                FROM pg_stat_activity
                WHERE datname = '{dbName}' AND pid <> pg_backend_pid();";

            await terminateCmd.ExecuteNonQueryAsync();
        }

        // Удаление базы данных
        using var dropCmd = connection.CreateCommand();
        dropCmd.CommandText = $"DROP DATABASE IF EXISTS \"{dbName}\"";

        await dropCmd.ExecuteNonQueryAsync();
    }
}