namespace Batura.Finbridge.TestTask.Application.Commands.Users;

/// <summary>
/// Информация для изменения баланса пользователя
/// </summary>
public sealed record BalanceUpdateInfo
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Изменение баланса (положительное или отрицательное)
    /// </summary>
    public decimal Delta { get; init; }
}