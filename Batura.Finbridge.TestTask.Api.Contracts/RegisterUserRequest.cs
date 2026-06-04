namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Запрос на регистрацию пользователя
/// </summary>
public sealed record RegisterUserRequest
{
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
    /// Дата рождения
    /// </summary>
    public DateOnly BirthDate { get; init; }

    /// <summary>
    /// Место рождения
    /// </summary>
    public string BirthPlace { get; init; } = string.Empty;
}