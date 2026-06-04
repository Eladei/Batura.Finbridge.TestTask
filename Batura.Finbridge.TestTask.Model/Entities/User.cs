using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Batura.Finbridge.TestTask.Model.Entities;

/// <summary>
/// Информация о пользователе
/// </summary>
[Index(nameof(FirstName), nameof(LastName), nameof(MiddleName), IsUnique = true)]
public class User : EntityBase
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    [Required]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия
    /// </summary>
    [Required]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Отчество
    /// </summary>
    [Required]
    public string MiddleName { get; set; } = string.Empty;

    /// <summary>
    /// Дата рождения
    /// </summary>
    [Required]
    public DateOnly BirthDate { get; set; }

    /// <summary>
    /// Место рождения
    /// </summary>
    [Required]
    public string BirthPlace { get; set; } = string.Empty;

    /// <summary>
    /// Баланс
    /// </summary>
    public decimal Balance { get; set; }

    public ICollection<BalanceHistory> BalanceHistories { get; set; } = [];
}