using Batura.Finbridge.TestTask.Application.Commands.Users;
using Shouldly;

namespace Batura.Finbridge.TestTask.Application.UnitTests.Commands;

/// <summary>
/// Юнит-тесты для ChangeBalanceCommand
/// </summary>
/// <see cref="ChangeBalanceCommand"/>
public sealed class ChangeBalanceCommandTests
{
    [Fact]
    public void Command_Should_Throw_ArgumentException_When_BalanceLimitProvider_Not_Defined()
    {
        // Arrange
        var id = Guid.NewGuid();
        var delta = 100m;

        // Act, Assert
        var exception = Assert.Throws<ArgumentNullException>(() 
            => new ChangeBalanceCommand(null!, id, delta));

        exception.ShouldNotBeNull();
        exception.ParamName.ShouldBe("balanceLimitProvider");
    }
}