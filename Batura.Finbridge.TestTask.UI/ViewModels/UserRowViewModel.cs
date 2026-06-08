namespace Batura.Finbridge.TestTask.UI.ViewModels;

/// <summary>
/// Модель представления информации о пользователе
/// </summary>
public sealed class UserRowViewModel
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Полное имя
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Дата рождения
    /// </summary>
    public DateOnly BirthDate { get; set; }

    /// <summary>
    /// Место рождения
    /// </summary>
    public string BirthPlace { get; set; } = string.Empty;

    /// <summary>
    /// Баланс
    /// </summary>
    public decimal Balance { get; set; }
}