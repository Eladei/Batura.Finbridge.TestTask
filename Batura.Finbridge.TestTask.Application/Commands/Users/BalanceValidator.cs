using Batura.Finbridge.TestTask.Application.Properties;

namespace Batura.Finbridge.TestTask.Application.Commands.Users;

/// <summary>
/// Валидатор баланса пользователя
/// </summary>
public static class BalanceValidator
{
    /// <summary>
    /// Проверяет корректность баланса пользователя
    /// </summary>
    /// <param name="currentBalance">Текущий баланс</param>
    /// <param name="newBalance">Новый баланс</param>
    /// <param name="balanceLimit">Лимит баланса</param>
    /// <param name="error">Сообщение об ошибке</param>
    /// <returns>Показатель корректности баланса пользователя</returns>
    public static bool Validate(decimal currentBalance, decimal newBalance, decimal balanceLimit, out string? error)
    {
        error = null;

        if (newBalance < 0)
        {
            error = Resources.BalanceCannotBeNegative;

            return false;
        }

        if (newBalance > balanceLimit)
        {
            error = Resources.BalanceCannotExceedLimit;

            return false;
        }

        return true;
    }
}