using Batura.Finbridge.TestTask.Application.Commands.Users;
using Batura.Finbridge.TestTask.Application.Properties;
using Shouldly;

namespace Batura.Finbridge.TestTask.Application.UnitTests.Commands;

/// <summary>
/// Юнит-тесты для RegisterBookCommand
/// </summary>
/// <see cref="RegisterBookCommand"/>
public sealed class RegisterUserCommandTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Command_Should_Throw_ArgumentException_When_FirstName_Not_Defined(string firstName)
    {
        // Arrange
        var id = Guid.NewGuid();
        var lastName = "Иванов";
        var middleName = "Иванович";
        var birthDate = new DateOnly(1985, 6, 6);
        var birthPlace = "Москва";

        // Act, Assert
        var exception = Assert.Throws<ArgumentException>(() 
            => new RegisterUserCommand(firstName, lastName, middleName, birthDate, birthPlace));

        exception.Message.ShouldBe(Resources.FirstNameNotDefined);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Command_Should_Throw_ArgumentException_When_LastName_Not_Defined(string lastName)
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "Иван";
        var middleName = "Иванович";
        var birthDate = new DateOnly(1985, 6, 6);
        var birthPlace = "Москва";

        // Act, Assert
        var exception = Assert.Throws<ArgumentException>(() 
            => new RegisterUserCommand(firstName, lastName, middleName, birthDate, birthPlace));

        exception.Message.ShouldBe(Resources.LastNameNotDefined);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Command_Should_Throw_ArgumentException_When_MiddleName_Not_Defined(string middleName)
    {
        // Arrange
        var firstName = "Иван";
        var lastName = "Иванов";
        var birthDate = new DateOnly(1985, 6, 6);
        var birthPlace = "Москва";

        // Act, Assert
        var exception = Assert.Throws<ArgumentException>(() 
            => new RegisterUserCommand(firstName, lastName, middleName, birthDate, birthPlace));

        exception.Message.ShouldBe(Resources.MiddleNameNotDefined);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Command_Should_Throw_ArgumentException_When_BirthPlace_Not_Defined(string birthPlace)
    {
        // Arrange
        var id = Guid.NewGuid();
        var firstName = "Иван";
        var lastName = "Иванов";
        var middleName = "Иванович";
        var birthDate = new DateOnly(1985, 6, 6);

        // Act, Assert
        var exception = Assert.Throws<ArgumentException>(() 
            => new RegisterUserCommand(firstName, lastName, middleName, birthDate, birthPlace));

        exception.Message.ShouldBe(Resources.BirthPlaceNotDefined);
    }
}