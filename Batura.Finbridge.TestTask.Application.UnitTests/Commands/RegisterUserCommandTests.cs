using Batura.Finbridge.TestTask.Application.Commands;
using Batura.Finbridge.TestTask.Application.Properties;
using Batura.Finbridge.TestTask.Model;
using Batura.Finbridge.TestTask.Model.Entities;
using Moq;
using Moq.EntityFrameworkCore;
using Shouldly;

namespace Batura.Finbridge.TestTask.Application.UnitTests.Commands;

/// <summary>
/// Юнит-тесты для RegisterBookCommand
/// </summary>
/// <see cref="RegisterBookCommand"/>
public sealed class RegisterUserCommandTests : EFUnitTestsBase<UserDbContext>
{
    private List<User> _users = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public Task Command_Should_Throw_ArgumentException_When_FirstName_Not_Defined(string firstName)
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = "Иванов",
            MiddleName = "Иванович",
            BirthDate = new DateOnly(1985, 6, 6),
            BirthPlace = "Москва"
        };

        _users.Add(user);

        // Act, Assert
        var exception = Assert.Throws<ArgumentException>(() => new RegisterUserCommand(
            user.FirstName, user.LastName, user.MiddleName, user.BirthDate, user.BirthPlace));

        exception.Message.ShouldBe(Resources.FirstNameNotDefined);

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public Task Command_Should_Throw_ArgumentException_When_LastName_Not_Defined(string lastName)
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Иван",
            LastName = lastName,
            MiddleName = "Иванович",
            BirthDate = new DateOnly(1985, 6, 6),
            BirthPlace = "Москва"
        };

        _users.Add(user);

        // Act, Assert
        var exception = Assert.Throws<ArgumentException>(() => new RegisterUserCommand(
            user.FirstName, user.LastName, user.MiddleName, user.BirthDate, user.BirthPlace));

        exception.Message.ShouldBe(Resources.LastNameNotDefined);

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public Task Command_Should_Throw_ArgumentException_When_MiddleName_Not_Defined(string middleName)
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Иван",
            LastName = "Иванов",
            MiddleName = middleName,
            BirthDate = new DateOnly(1985, 6, 6),
            BirthPlace = "Москва"
        };

        _users.Add(user);

        // Act, Assert
        var exception = Assert.Throws<ArgumentException>(() => new RegisterUserCommand(
            user.FirstName, user.LastName, user.MiddleName, user.BirthDate, user.BirthPlace));

        exception.Message.ShouldBe(Resources.MiddleNameNotDefined);

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public Task Command_Should_Throw_ArgumentException_When_BirthPlace_Not_Defined(string birthPlace)
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Иван",
            LastName = "Иванов",
            MiddleName = "Иванович",
            BirthDate = new DateOnly(1985, 6, 6),
            BirthPlace = birthPlace
        };

        _users.Add(user);

        // Act, Assert
        var exception = Assert.Throws<ArgumentException>(() => new RegisterUserCommand(
            user.FirstName, user.LastName, user.MiddleName, user.BirthDate, user.BirthPlace));

        exception.Message.ShouldBe(Resources.BirthPlaceNotDefined);

        return Task.CompletedTask;
    }

    protected override UserDbContext SetUpDbContext(Mock<UserDbContext> contextMock)
    {
        contextMock.Setup(s => s.Users).ReturnsDbSet(_users);

        return contextMock.Object;
    }
}