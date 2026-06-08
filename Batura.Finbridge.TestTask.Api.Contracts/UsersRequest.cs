namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Запрос на получение зарегистрированных пользователей
/// </summary>
public sealed record UsersRequest
{
    /// <summary>
    /// Номер страницы
    /// </summary>
    public uint Page { get; init; }

    /// <summary>
    /// Количество пользователей на странице
    /// </summary>
    public uint UsersPerPage { get; init; }
}