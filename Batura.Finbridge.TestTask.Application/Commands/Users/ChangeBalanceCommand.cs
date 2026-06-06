using Batura.Finbridge.TestTask.Application.Exceptions;
using Batura.Finbridge.TestTask.Application.IntegrationEvents;
using Batura.Finbridge.TestTask.Application.Properties;
using Batura.Finbridge.TestTask.Model;
using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Commands.Users;

/// <summary>
/// Изменяет баланс пользователя
/// </summary>
public sealed class ChangeBalanceCommand : CommandBase<UserDbContext>
{
    private readonly Guid _userId;
    private readonly decimal _delta;
    private readonly BalanceValidator _balanceValidator;

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
        _balanceValidator = new BalanceValidator(balanceLimitProvider);

        _userId = userId;
        _delta = delta;
    }

    /// </inheritdoc>
    public override async Task ExecuteAsync(UserDbContext context, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == _userId, cancellationToken)
            ?? throw new OperationLogicException(Resources.UserNotFoundById, _userId);

        var newBalance = user.Balance + _delta;

        var isBalanceValid = _balanceValidator.Validate(user.Balance, newBalance, out var error);

        if (!isBalanceValid)
            throw new OperationLogicException(error!);

        var balanceBefore = user.Balance;
        user.Balance = newBalance;

        var balanceWasChangedEvent = new UserBalanceWasChangedIntegrationEvent(
            user.Id, user.FirstName, user.LastName, user.MiddleName, 
            balanceBefore, newBalance, DateTime.UtcNow);

        var outboxEvent = IntegrationEventConverter.Convert(balanceWasChangedEvent);

        context.IntegrationEvents.Add(outboxEvent);
    }
}