namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Ответ на запрос получения истории изменения баланса пользователей
/// </summary>
public sealed record BalanceHistoryResponse
{
    /// <summary>
    /// История изменения баланса пользователей
    /// </summary>
    public BalanceHistoryItem[] History { get; init; } = [];

    /// <summary>
    /// Общее количество страниц
    /// </summary>
    public int TotalPages { get; init; }
}