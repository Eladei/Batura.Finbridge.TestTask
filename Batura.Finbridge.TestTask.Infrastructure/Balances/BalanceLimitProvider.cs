using Batura.Finbridge.TestTask.Application.Commands.Users;
using Batura.Finbridge.TestTask.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace Batura.Finbridge.TestTask.Infrastructure.Balances;

/// <summary>
/// Провайдер лимита баланса пользователя
/// </summary>
public class BalanceLimitProvider : IBalanceLimitProvider
{
    private readonly decimal _balanceLimit;

    /// <summary>
    /// Создает объект класса <see cref="BalanceLimitProvider"/>
    /// </summary>
    /// <param name="options">Настройки баланса пользователей</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public BalanceLimitProvider(IOptions<BalanceOptions> options) 
    {
        var settings = options.Value
            ?? throw new ArgumentNullException(nameof(options));

        if (settings.BalanceLimit <= 0)
            throw new ArgumentException(
                "Максимальное значение баланса пользователя должно быть больше 0");

        _balanceLimit = settings.BalanceLimit;
    }

    /// <inheritdoc/>
    public decimal GetBalanceLimit() => _balanceLimit;
}