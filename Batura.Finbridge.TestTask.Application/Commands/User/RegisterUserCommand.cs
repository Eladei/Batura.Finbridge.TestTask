using Batura.Finbridge.TestTask.Application.Exceptions;
using Batura.Finbridge.TestTask.Application.Properties;
using Batura.Finbridge.TestTask.Model;
using Batura.Finbridge.TestTask.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Batura.Finbridge.TestTask.Application.Commands;

/// <summary>
/// Добавляет нового пользователя в систему
/// </summary>
public sealed class RegisterUserCommand : CommandBase<UserDbContext>
{
    private readonly string _firstName;
    private readonly string _lastName;
    private readonly string _middleName;
    private readonly DateOnly _birthDate;
    private readonly string _birthPlace;

    /// <summary>
    /// Создает объект класса RegisterUserCommand
    /// </summary>
    /// <param name="firstName">Имя</param>
    /// <param name="lastName">Фамилия</param>
    /// <param name="middleName">Отчество</param>
    /// <param name="birthDate">Дата рождения</param>
    /// <param name="birthPlace">Место рождения</param>
    /// <exception cref="ArgumentException"></exception>
    public RegisterUserCommand(
        string firstName, string lastName, string middleName, 
        DateOnly birthDate, string birthPlace)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException(Resources.FirstNameNotDefined);

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException(Resources.LastNameNotDefined);

        if (string.IsNullOrWhiteSpace(middleName))
            throw new ArgumentException(Resources.MiddleNameNotDefined);

        if (string.IsNullOrWhiteSpace(birthPlace))
            throw new ArgumentException(Resources.BirthPlaceNotDefined);

        _firstName = firstName;
        _lastName = lastName;
        _middleName = middleName;
        _birthDate = birthDate;
        _birthPlace = birthPlace;
    }

    /// </inheritdoc>
    public override async Task BeforeExecuteAsync(UserDbContext context, CancellationToken cancellationToken)
    {
        var userExists = await context.Users
            .AnyAsync(
                s => s.FirstName == _firstName
                    && s.LastName == _lastName
                    && s.MiddleName == _middleName
                    && s.BirthDate == _birthDate,
                cancellationToken);

        var fullName = $"{_firstName} {_lastName} {_middleName}";

        if (userExists)
            throw new OperationLogicException(
                Resources.UserAlreadyExists, fullName, _birthDate);
    }

    /// </inheritdoc>
    public override async Task ExecuteAsync(UserDbContext context, CancellationToken cancellationToken)
    {
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = _firstName,
            LastName = _lastName,
            MiddleName = _middleName,
            BirthDate = _birthDate,
            BirthPlace = _birthPlace
        };

        await context.Users.AddAsync(newUser, cancellationToken);
    }
}