namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Информация для изменения баланса пользователя
/// </summary>
public sealed record BalanceUpdateItem
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