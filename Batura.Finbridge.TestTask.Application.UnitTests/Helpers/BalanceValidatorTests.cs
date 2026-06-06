using Batura.Finbridge.TestTask.Application.Commands.Users;
using Batura.Finbridge.TestTask.Application.Properties;
using Shouldly;

namespace Batura.Finbridge.TestTask.Application.UnitTests.Commands;

/// <summary>
/// Юнит-тесты для BalanceValidator
/// </summary>
/// <see cref="BalanceValidator"/>
public sealed class BalanceValidatorTests
{
    [Fact]
    public void Validator_Should_Return_True_When_NewBalance_Greater_Than_Zero_And_Not_Exceeds_Limit()
    {
        // Arrange
        decimal newBalance = 150m;
        decimal balanceLimit = 200m;

        // Act, Assert
        var result = BalanceValidator.Validate(newBalance, balanceLimit, out _);

        result.ShouldBe(true);
    }

    [Fact]
    public void Validator_Should_Return_False_When_NewBalance_Lower_Than_Zero()
    {
        // Arrange
        decimal newBalance = -50m;
        decimal balanceLimit = 200m;

        var expectedError = Resources.BalanceCannotBeNegative;

        // Act, Assert
        var result = BalanceValidator.Validate(newBalance, balanceLimit, out var error);

        result.ShouldBe(false);
        error.ShouldBe(expectedError);
    }

    [Fact]
    public void Validator_Should_Return_False_When_NewBalance_Exceeds_Limit()
    {
        // Arrange
        decimal newBalance = 400m;
        decimal balanceLimit = 200m;

        var expectedError = Resources.BalanceCannotExceedLimit;

        // Act, Assert
        var result = BalanceValidator.Validate(newBalance, balanceLimit, out var error);

        result.ShouldBe(false);
        error.ShouldBe(expectedError);
    }
}