namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Запрос на получение истории изменения баланса пользователей
/// </summary>
public sealed record BalanceHistoryRequest
{
    /// <summary>
    /// Номер страницы
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Количество записей на странице
    /// </summary>
    public int HistoryItemsPerPage { get; init; }
}