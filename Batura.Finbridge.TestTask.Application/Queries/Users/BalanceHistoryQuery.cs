using Batura.Finbridge.TestTask.Model;
using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Queries.Users;

/// <summary>
/// Запрос информации об изменении баланса пользователей
/// </summary>
public sealed class BalanceHistoryQuery : PageQueryBase<UserDbContext, BalanceHistoryItemReadModel>
{
    /// <summary>
    /// Создает объект класса <see cref="BalanceHistoryQuery"/>
    /// </summary>
    /// <param name="usersPerPage">Число записей истории баланса на странице</param>
    /// <param name="page">Target page number</param>
    public BalanceHistoryQuery(uint usersPerPage, uint page)
        : base(usersPerPage, page) { }

    protected override async Task<IEnumerable<BalanceHistoryItemReadModel>> PerformAsync(
        UserDbContext context,
        CancellationToken cancellationToken)
    {
        var query = context.BalanceHistories
            .OrderByDescending(s => s.ChangedAtUtc)
            .Skip((int)ElementsToSkip);

        if (_elementsPerPage.HasValue)
            query = query.Take((int)_elementsPerPage);

        var balanceHistoryInfos = await query.Select(s => new BalanceHistoryItemReadModel
        {
            Id = s.Id,
            UserId = s.UserId,
            FirstName = s.User.FirstName,
            LastName = s.User.LastName,
            MiddleName = s.User.MiddleName,
            BalanceBefore = s.BalanceBefore,
            BalanceAfter = s.BalanceAfter,
            ChangedAtUtc = s.ChangedAtUtc
        }).ToArrayAsync(cancellationToken);

        return balanceHistoryInfos;
    }

    /// <inheritdoc />
    protected override async Task<uint> GetAllElementsCount(
        UserDbContext context,
        CancellationToken cancellationToken)
        => (uint)await context.BalanceHistories.CountAsync(cancellationToken);
}