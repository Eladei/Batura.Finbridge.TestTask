using System.ComponentModel.DataAnnotations;

namespace Batura.Finbridge.TestTask.Model.Entities;

/// <summary>
/// Базовый класс сущности базы данных
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Версия строки для оптимистичной блокировки
    /// </summary>
    public uint Version { get; set; }

    /// <summary>
    /// Дата и время создания в UTC
    /// </summary>
    [Required]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Последняя дата и время изменения в UTC
    /// </summary>
    public DateTime? ModifiedAtUtc { get; set; }
}