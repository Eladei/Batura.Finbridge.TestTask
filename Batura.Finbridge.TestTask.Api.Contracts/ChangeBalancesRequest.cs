namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Запрос на изменение балансов пользователей
/// </summary>
public sealed record ChangeBalancesRequest
{
    /// <summary>
    /// Информация для изменения баланса пользователей
    /// </summary>
    public BalanceUpdateItem[] Items { get; init; } = [];
}