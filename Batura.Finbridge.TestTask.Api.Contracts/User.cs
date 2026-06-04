namespace Batura.Finbridge.TestTask.Api.Contracts;

/// <summary>
/// Информация о зарегистрированном пользователе
/// </summary>
public sealed record User
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
}