using Batura.Finbridge.TestTask.Application.Exceptions;
using Batura.Finbridge.TestTask.Application.IntegrationEvents;
using Batura.Finbridge.TestTask.Application.Properties;
using Batura.Finbridge.TestTask.Model;
using Batura.Finbridge.TestTask.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Commands.Users;

/// <summary>
/// Изменяет баланс нескольких пользователей
/// </summary>
public sealed class ChangeBalancesCommand : CommandBase<UserDbContext>
{
    private readonly Dictionary<Guid, BalanceUpdateInfo> _updateInfosByUserId;
    private readonly IBalanceLimitProvider _balanceLimitProvider;

    /// <summary>
    /// Создает объект класса <see cref="ChangeBalancesCommand"/>
    /// </summary>
    /// <param name="balanceLimitProvider">Провайдер лимита баланса пользователя</param>
    /// <param name="balanceUpdateInfos">Информация для изменения баланса пользователей</param>
    public ChangeBalancesCommand(
        IBalanceLimitProvider balanceLimitProvider,
        BalanceUpdateInfo[] balanceUpdateInfos)
    {
        _balanceLimitProvider = balanceLimitProvider
            ?? throw new ArgumentNullException(nameof(balanceLimitProvider));

        ArgumentNullException.ThrowIfNull(balanceUpdateInfos, nameof(balanceUpdateInfos));

        ThrowIfUsersDublicated(balanceUpdateInfos);

        _updateInfosByUserId = balanceUpdateInfos.ToDictionary(x => x.UserId);
    }

    /// <inheritdoc/>
    public override async Task ExecuteAsync(UserDbContext context, CancellationToken cancellationToken)
    {
        var userIds = _updateInfosByUserId.Keys.ToArray();

        var users = await context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);

        ThrowIfUsersNotFound(users, userIds);

        var balanceLimit = _balanceLimitProvider.GetBalanceLimit();

        var operationDate = DateTime.UtcNow;

        foreach (var user in users)
        {
            var delta = _updateInfosByUserId[user.Id].Delta;

            var newBalance = user.Balance + delta;

            var validationResult = BalanceValidator.Validate(newBalance, balanceLimit);

            if (validationResult == BalanceValidationResult.NegativeBalance)
                throw new OperationLogicException(Resources.BalanceCannotBeNegative, user.Id, newBalance);

            if (validationResult == BalanceValidationResult.BalanceLimitExceeded)
                throw new OperationLogicException(
                    Resources.BalanceCannotExceedLimit, user.Id, newBalance, balanceLimit);

            var balanceBefore = user.Balance;
            user.Balance = newBalance;

            context.BalanceHistories.Add(new BalanceHistory 
            { 
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BalanceBefore = balanceBefore,
                BalanceAfter = newBalance,
                ChangedAtUtc = operationDate
            });

            var balanceWasChangedEvent = new UserBalanceWasChangedIntegrationEvent(
                user.Id, user.FirstName, user.LastName, user.MiddleName,
                balanceBefore, newBalance, operationDate);

            var outboxEvent = IntegrationEventConverter.Convert(balanceWasChangedEvent);

            context.IntegrationEvents.Add(outboxEvent);
        }
    }

    private static void ThrowIfUsersDublicated(BalanceUpdateInfo[] balanceUpdateInfos) 
    { 
        var totalUsersCount = balanceUpdateInfos.Length;

        var distinctUsersCount = balanceUpdateInfos
            .Select(b => b.UserId)
            .Distinct()
            .Count();

        if (totalUsersCount != distinctUsersCount)
            throw new OperationLogicException(Resources.UsersDublicatedInBalanceUpdate);
    }

    private static void ThrowIfUsersNotFound(List<User> users, Guid[] selectedUserIds) 
    {
        var foundUsersIds = users.Select(u => u.Id).ToHashSet();

        var notFoundUserIds = selectedUserIds
            .Except(foundUsersIds)
            .ToArray();

        if (notFoundUserIds.Any())
        {
            var ids = string.Join(", ", notFoundUserIds);

            throw new OperationLogicException(Resources.UsersNotFoundByIds, ids);
        }
    }
}