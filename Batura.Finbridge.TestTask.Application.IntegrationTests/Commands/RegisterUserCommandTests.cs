using Batura.Finbridge.TestTask.Application.Commands.Users;
using Batura.Finbridge.TestTask.Application.Exceptions;
using Batura.Finbridge.TestTask.Model;
using Batura.Finbridge.TestTask.Model.Entities;
using Batura.Finbridge.TestTask.Application.Properties;
using Shouldly;

namespace Batura.Finbridge.TestTask.Application.IntegrationTests.Commands;

/// <summary>
/// Интеграционные тесты для RegisterBookCommand
/// </summary>
/// <see cref="RegisterUserCommand"/>
public sealed class RegisterUserCommandTests : NpgsqlIntegrationTestsBase<UserDbContext>
{
    public RegisterUserCommandTests(NpgsqlConnectionParams serverConnectionParams)
        : base(serverConnectionParams, opts => new UserDbContext(opts)) { }

    [Fact]
    public async Task Command_Should_Throw_OperationLogicException_When_User_Already_Exists()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Иван",
            LastName = "Иванов",
            MiddleName = "Иванович",
            BirthDate = new DateOnly(1985, 6, 6),
            BirthPlace = "Москва"
        };

        using var context = CreateContext();

        context.Users.Add(user);

        await context.SaveChangesAsync(CancellationToken.None);

        var fullName = $"{user.FirstName} {user.LastName} {user.MiddleName}";

        var expectedError = string.Format(Resources.UserAlreadyExists, fullName, user.BirthDate);

        // Act, Assert
        var command = new RegisterUserCommand(
            user.FirstName, user.LastName, user.MiddleName, user.BirthDate, user.BirthPlace);

        var exception = await Assert.ThrowsAsync<OperationLogicException>(
            () => command.BeforeExecuteAsync(context, CancellationToken.None));

        exception.ShouldNotBeNull();
        exception.Message.ShouldBe(expectedError);
    }

    [Fact]
    public async Task Command_Should_Register_New_User()
    {
        // Arrange
        var firstName = "Иван";
        var lastName = "Иванов";
        var middleName = "Иванович";
        var birthDate = new DateOnly(1985, 6, 6);
        var birthPlace = "Москва";

        // Act, Assert
        var command = new RegisterUserCommand(
            firstName, lastName, middleName, birthDate, birthPlace);

        using var context = CreateContext();

        // Act
        await command.BeforeExecuteAsync(context, CancellationToken.None);
        await command.ExecuteAsync(context, CancellationToken.None);

        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var addedUser = context.Users.FirstOrDefault();

        addedUser.ShouldNotBeNull();
        addedUser.FirstName.ShouldBe(firstName);
        addedUser.LastName.ShouldBe(lastName);
        addedUser.MiddleName.ShouldBe(middleName);
        addedUser.BirthDate.ShouldBe(birthDate);
        addedUser.BirthPlace.ShouldBe(birthPlace);
    }
}