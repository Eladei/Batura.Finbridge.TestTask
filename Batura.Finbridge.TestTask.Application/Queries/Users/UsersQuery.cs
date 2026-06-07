using Batura.Finbridge.TestTask.Model;
using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Queries.Users;

/// <summary>
/// Запрос информации о пользователях
/// </summary>
public sealed class UsersQuery : EfPageQueryBase<UserDbContext, UserReadModel>
{
    /// <summary>
    /// Создает объект класса <see cref="UsersQuery"/>
    /// </summary>
    /// <param name="usersPerPage">Число пользователей на странице</param>
    /// <param name="page">Target page number</param>
    public UsersQuery(uint usersPerPage, uint page)
        : base(usersPerPage, page) { }

    protected override async Task<IEnumerable<UserReadModel>> PerformAsync(
        UserDbContext context,
        CancellationToken cancellationToken)
    {
        var query = context.Users
            .OrderByDescending(s => s.CreatedAtUtc)
            .Skip((int)ElementsToSkip);

        if (_elementsPerPage.HasValue)
            query = query.Take((int)_elementsPerPage);

        var bookInfos = await query.Select(s => new UserReadModel
        {
            Id = s.Id,
            FirstName = s.FirstName,
            LastName = s.LastName,
            MiddleName = s.MiddleName,
            BirthDate = s.BirthDate,
            BirthPlace = s.BirthPlace,
            Balance = s.Balance,
            RegisteredAtUtc = s.CreatedAtUtc
        }).ToArrayAsync(cancellationToken);

        return bookInfos;
    }

    /// <inheritdoc />
    protected override async Task<uint> GetAllElementsCount(
        UserDbContext context,
        CancellationToken cancellationToken)
        => (uint)await context.Users.CountAsync(cancellationToken);
}