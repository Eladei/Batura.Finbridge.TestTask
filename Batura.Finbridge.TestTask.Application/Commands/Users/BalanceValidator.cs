using Batura.Finbridge.TestTask.Application.Properties;

namespace Batura.Finbridge.TestTask.Application.Commands.Users;

/// <summary>
/// Валидатор баланса пользователя
/// </summary>
public sealed class BalanceValidator
{
    private readonly IBalanceLimitProvider _balanceLimitProvider;

    /// <summary>
    /// Создает объект класса <see cref="BalanceValidator"/>
    /// </summary>
    /// <param name="balanceLimitProvider">Провайдер лимита баланса пользователя</param>
    /// <exception cref="ArgumentNullException"></exception>
    public BalanceValidator(IBalanceLimitProvider balanceLimitProvider)
    {
        _balanceLimitProvider = balanceLimitProvider
            ?? throw new ArgumentNullException(nameof(balanceLimitProvider));
    }

    /// <summary>
    /// Проверяет, что новый баланс не превышает лимит и не уходит в минус
    /// </summary>
    /// <param name="currentBalance">Текущий баланс</param>
    /// <param name="newBalance">Новый баланс</param>
    public bool Validate(decimal currentBalance, decimal newBalance, out string? error)
    {
        error = null;

        if (newBalance < 0)
        {
            error = Resources.BalanceCannotBeNegative;

            return false;
        }

        var balanceLimit = _balanceLimitProvider.GetBalanceLimit();

        if (newBalance > balanceLimit)
        {
            error = Resources.BalanceCannotExceedLimit;

            return false;
        }

        return true;
    }
}