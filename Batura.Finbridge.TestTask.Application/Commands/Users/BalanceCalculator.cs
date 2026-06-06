using Batura.Finbridge.TestTask.Application.Exceptions;

namespace Batura.Finbridge.TestTask.Application.Commands.Users;

/// <summary>
/// Класс для расчета нового баланса пользователя с учетом валидации
/// </summary>
public static class BalanceCalculator
{
    /// <summary>
    /// Вычисляет новый баланс пользователя
    /// </summary>
    /// <param name="currentBalance">Текущий баланс пользователя</param>
    /// <param name="delta">Изменение баланса</param>
    /// <param name="balanceLimit">Лимит баланса</param>
    /// <returns>Новый баланс пользователя</returns>
    /// <exception cref="OperationLogicException"></exception>
    public static decimal CalculateBalance(decimal currentBalance, decimal delta, decimal balanceLimit)
    {
        var newBalance = currentBalance + delta;

        var isBalanceValid = BalanceValidator.Validate(
            currentBalance, newBalance, balanceLimit, out var error);

        if (!isBalanceValid)
            throw new OperationLogicException(error!);

        return newBalance;
    }
}