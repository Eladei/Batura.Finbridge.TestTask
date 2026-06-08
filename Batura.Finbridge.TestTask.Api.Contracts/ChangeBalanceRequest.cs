namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Запрос на изменение баланса пользователя
/// </summary>
public sealed record ChangeBalanceRequest
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Изменение баланса (положительное или отрицательное)
    /// </summary>
    public required decimal Delta { get; init; }
}