using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Batura.Finbridge.TestTask.Model;

/// <summary>
/// Фабрика для генерации контекста БД исключительно во время генерации миграций (Design-Time)
/// </summary>
public sealed class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();

        optionsBuilder.UseNpgsql();

        return new UserDbContext(optionsBuilder.Options);
    }
}