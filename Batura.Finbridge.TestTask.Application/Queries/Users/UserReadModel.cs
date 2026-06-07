namespace Batura.Finbridge.TestTask.Application.Queries.Users;

/// <summary>
/// Модель данных для чтения информации о пользователе
/// </summary>
public record UserReadModel
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; init; }

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

    /// <summary>
    /// Баланс
    /// </summary>
    public decimal Balance { get; init; }

    /// <summary>
    /// Время регистрации пользователя в UTC
    /// </summary>
    public DateTime RegisteredAtUtc { get; init; }
}