using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Batura.Finbridge.TestTask.Model.Entities;

/// <summary>
/// Информация об изменении баланса пользователя
/// </summary>
public class BalanceHistory : EntityBase
{
    /// <summary>
    /// Идентификатор записи об изменении баланса
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    [Required]
    public Guid UserId { get; init; }

    /// <summary>
    /// Баланс до изменения
    /// </summary>
    public decimal BalanceBefore { get; init; }

    /// <summary>
    /// Баланс после изменения
    /// </summary>
    public decimal BalanceAfter { get; init; }

    /// <summary>
    /// Время изменения баланса
    /// </summary>
    [Required]
    public DateTime ChangedAt { get; init; }

    /// <summary>
    /// Пользователь, которому принадлежит баланс
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public User User { get; init; } = null!;
}