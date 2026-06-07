namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Ответ на запрос получения зарегистрированных пользователей
/// </summary>
public sealed record UsersResponse
{
    /// <summary>
    /// Зарегистрированные пользователи
    /// </summary>
    public User[] Users { get; init; } = [];

    /// <summary>
    /// Общее количество страниц
    /// </summary>
    public uint TotalPages { get; init; }
}