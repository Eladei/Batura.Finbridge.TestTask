namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Запрос на регистрацию пользователя
/// </summary>
public sealed record RegisterUserRequest
{
    /// <summary>
    /// Имя
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// Фамилия
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Отчество
    /// </summary>
    public required string MiddleName { get; init; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    public required DateOnly BirthDate { get; init; }

    /// <summary>
    /// Место рождения
    /// </summary>
    public required string BirthPlace { get; init; }
}