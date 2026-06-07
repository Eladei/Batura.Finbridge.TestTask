namespace Batura.Finbridge.TestTask.Application.Queries.Users;

/// <summary>
/// Информация об изменении баланса пользователя
/// </summary>
public sealed record BalanceHistoryItemReadModel
{
    /// <summary>
    /// Идентификатор изменения баланса
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Фамилия
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Отчество
    /// </summary>
    public string MiddleName { get; init; } = string.Empty;

    /// <summary>
    /// Баланс до изменения
    /// </summary>
    public decimal BalanceBefore { get; init; }

    /// <summary>
    /// Баланс после изменения
    /// </summary>
    public decimal BalanceAfter { get; init; }

    /// <summary>
    /// Время изменения баланса в UTC
    /// </summary>
    public DateTime ChangedAtUtc { get; init; }
}