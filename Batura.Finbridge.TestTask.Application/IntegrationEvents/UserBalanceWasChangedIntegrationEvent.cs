namespace Batura.Finbridge.TestTask.Application.IntegrationEvents;

/// <summary>
/// Информация об изменении баланса пользователя
/// </summary>
public sealed class UserBalanceWasChangedIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// Создает объект класса <see cref="UserBalanceWasChangedIntegrationEvent"/>
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="firstName">Имя пользователя</param>
    /// <param name="lastName">Фамилия пользователя</param>
    /// <param name="middleName">Отчество пользователя</param>
    /// <param name="balanceBefore">Баланс до изменения</param>
    /// <param name="balanceAfter">Баланс после изменения</param>
    /// <param name="changedAt">Время изменения баланса</param>
    public UserBalanceWasChangedIntegrationEvent(
        Guid userId, string firstName, string lastName, string middleName, 
        decimal balanceBefore, decimal balanceAfter, DateTime changedAt) : base(userId)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        BalanceBefore = balanceBefore;
        BalanceAfter = balanceAfter;
        ChangedAt = changedAt;
    }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; }

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
    public DateTime ChangedAt { get; init; }
}