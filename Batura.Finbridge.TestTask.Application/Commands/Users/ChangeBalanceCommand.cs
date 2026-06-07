using Batura.Finbridge.TestTask.Application.Exceptions;
using Batura.Finbridge.TestTask.Application.IntegrationEvents;
using Batura.Finbridge.TestTask.Application.Properties;
using Batura.Finbridge.TestTask.Model;
using Batura.Finbridge.TestTask.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Commands.Users;

/// <summary>
/// Изменяет баланс пользователя
/// </summary>
public sealed class ChangeBalanceCommand : CommandBase<UserDbContext>
{
    private readonly Guid _userId;
    private readonly decimal _delta;
    private readonly IBalanceLimitProvider _balanceLimitProvider;

    /// <summary>
    /// Создает объект класса <see cref="ChangeBalanceCommand"/>
    /// </summary>
    /// <param name="balanceLimitProvider">Провайдер лимита баланса пользователя</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="delta">Изменение баланса</param>
    public ChangeBalanceCommand(
        IBalanceLimitProvider balanceLimitProvider,
        Guid userId, decimal delta)
    {
        _balanceLimitProvider = balanceLimitProvider
            ?? throw new ArgumentNullException(nameof(balanceLimitProvider));

        _userId = userId;
        _delta = delta;
    }

    /// <inheritdoc/>
    public override async Task ExecuteAsync(UserDbContext context, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == _userId, cancellationToken)
            ?? throw new OperationLogicException(Resources.UserNotFoundById, _userId);

        var r = user.Version;

        var balanceLimit = _balanceLimitProvider.GetBalanceLimit();

        var newBalance = user.Balance + _delta;

        var validationResult = BalanceValidator.Validate(newBalance, balanceLimit);

        if (validationResult == BalanceValidationResult.NegativeBalance)
            throw new OperationLogicException(Resources.BalanceCannotBeNegative, _userId, newBalance);

        if (validationResult == BalanceValidationResult.BalanceLimitExceeded)
            throw new OperationLogicException(
                Resources.BalanceCannotExceedLimit, _userId, newBalance, balanceLimit);

        var balanceBefore = user.Balance;
        user.Balance = newBalance;

        var operationDate = DateTime.UtcNow;

        context.BalanceHistories.Add(new BalanceHistory
        {
            Id = Guid.NewGuid(),
            UserId = _userId,
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