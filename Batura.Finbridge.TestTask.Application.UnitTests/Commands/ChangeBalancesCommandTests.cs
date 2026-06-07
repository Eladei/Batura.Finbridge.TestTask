using Batura.Finbridge.TestTask.Application.Commands.Users;
using Batura.Finbridge.TestTask.Application.Exceptions;
using Batura.Finbridge.TestTask.Application.Properties;
using Moq;
using Shouldly;

namespace Batura.Finbridge.TestTask.Application.UnitTests.Commands;

/// <summary>
/// Юнит-тесты для ChangeBalancesCommand
/// </summary>
/// <see cref="ChangeBalancesCommand"/>
public sealed class ChangeBalancesCommandTests
{
    [Fact]
    public void Command_Should_Throw_ArgumentException_When_BalanceLimitProvider_Not_Defined()
    {
        // Arrange
        var balanceUpdateInfos = new BalanceUpdateInfo[] {
            new() 
            {
                UserId = Guid.NewGuid(), 
                Delta = 100m
            }
        };

        // Act, Assert
        var exception = Assert.Throws<ArgumentNullException>(() 
            => new ChangeBalancesCommand(null!, balanceUpdateInfos));

        exception.ParamName.ShouldBe("balanceLimitProvider");
    }

    [Fact]
    public void Command_Should_Throw_OperationLogicException_When_Users_Dublicated()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var balanceUpdateInfos = new BalanceUpdateInfo[] {
            new()
            {
                UserId = userId,
                Delta = 100m
            },
            new()
            {
                UserId = userId,
                Delta = 300m
            }
        };

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();

        // Act, Assert
        var exception = Assert.Throws<OperationLogicException>(()
            => new ChangeBalancesCommand(balanceLimitProvider.Object, balanceUpdateInfos));

        exception.Message.ShouldBe(Resources.UsersDublicatedInBalanceUpdate);
    }
}