using Batura.Finbridge.TestTask.Application.Commands.Users;
using Batura.Finbridge.TestTask.Application.Exceptions;
using Batura.Finbridge.TestTask.Application.IntegrationEvents;
using Batura.Finbridge.TestTask.Application.Properties;
using Batura.Finbridge.TestTask.Model;
using Moq;
using Shouldly;
using User = Batura.Finbridge.TestTask.Model.Entities.User;

namespace Batura.Finbridge.TestTask.Application.IntegrationTests.Commands;

/// <summary>
/// Интеграционные тесты для ChangeBalanceCommand
/// </summary>
/// <see cref="ChangeBalanceCommand"/>
public sealed class ChangeBalanceCommandTests : NpgsqlIntegrationTestsBase<UserDbContext>
{
    public ChangeBalanceCommandTests(NpgsqlConnectionParams serverConnectionParams)
        : base(serverConnectionParams, opts => new UserDbContext(opts)) { }

    [Fact]
    public async Task Command_Should_Throw_OperationLogicException_When_User_Not_Found_By_Id()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var delta = 100m;
        var expectedError = string.Format(Resources.UserNotFoundById, userId);

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();

        using var context = CreateContext();

        // Act, Assert
        var command = new ChangeBalanceCommand(balanceLimitProvider.Object, userId, delta);

        var exception = await Assert.ThrowsAsync<OperationLogicException>(
            () => command.ExecuteAsync(context, CancellationToken.None));

        exception.Message.ShouldBe(expectedError);
    }

    [Fact]
    public async Task Command_Should_Throw_OperationLogicException_When_New_Balance_Negative()
    {
        // Arrange
        var delta = -100m;
        var balanceBefore = 30m;
        var expectedBalance = -70m;
        var balanceLimit = 50m;

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();
        balanceLimitProvider.Setup(x => x.GetBalanceLimit()).Returns(balanceLimit);

        using var context = CreateContext();

        var newUser = CreateUser();
        newUser.Balance = balanceBefore;
        context.Users.Add(newUser);

        await context.SaveChangesAsync(CancellationToken.None);

        var expectedError = string.Format(
            Resources.BalanceCannotBeNegative, newUser.Id, expectedBalance);

        // Act
        var command = new ChangeBalanceCommand(balanceLimitProvider.Object, newUser.Id, delta);

        // Assert
        var exception = await Assert.ThrowsAsync<OperationLogicException>(
            () => command.ExecuteAsync(context, CancellationToken.None));

        exception.ShouldNotBeNull();
        exception.Message.ShouldBe(expectedError);
    }

    [Fact]
    public async Task Command_Should_Throw_OperationLogicException_When_Balance_Limit_Exceeded()
    {
        // Arrange
        var delta = 100m;
        var balanceBefore = 30m;
        var expectedBalance = 130m;
        var balanceLimit = 50m;

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();
        balanceLimitProvider.Setup(x => x.GetBalanceLimit()).Returns(balanceLimit);

        using var context = CreateContext();

        var newUser = CreateUser();
        newUser.Balance = balanceBefore;
        context.Users.Add(newUser);

        await context.SaveChangesAsync(CancellationToken.None);

        var expectedError = string.Format(
            Resources.BalanceCannotExceedLimit, newUser.Id, expectedBalance, balanceLimit);

        // Act
        var command = new ChangeBalanceCommand(balanceLimitProvider.Object, newUser.Id, delta);

        // Assert
        var exception = await Assert.ThrowsAsync<OperationLogicException>(
            () => command.ExecuteAsync(context, CancellationToken.None));

        exception.ShouldNotBeNull();
        exception.Message.ShouldBe(expectedError);
    }

    [Fact]
    public async Task Command_Should_Change_Balance_Correctly()
    {
        // Arrange
        var delta = 100m;
        var balanceBefore = 30m;
        var expectedBalance = 130m;
        var balanceLimit = 200m;

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();
        balanceLimitProvider.Setup(x => x.GetBalanceLimit()).Returns(balanceLimit);

        using var context = CreateContext();

        var newUser = CreateUser();
        newUser.Balance = balanceBefore;
        context.Users.Add(newUser);

        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var command = new ChangeBalanceCommand(
            balanceLimitProvider.Object, newUser.Id, delta);

        await command.ExecuteAsync(context, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var user = context.Users.FirstOrDefault(u => u.Id == newUser.Id);

        user.ShouldNotBeNull();
        user.Balance.ShouldBe(expectedBalance);
    }

    [Fact]
    public async Task Command_Should_Save_Balance_History_Correctly()
    {
        // Arrange
        var delta = 100m;
        var balanceBefore = 30m;
        var expectedBalance = 130m;
        var balanceLimit = 200m;

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();
        balanceLimitProvider.Setup(x => x.GetBalanceLimit()).Returns(balanceLimit);

        using var context = CreateContext();

        var newUser = CreateUser();
        newUser.Balance = balanceBefore;
        context.Users.Add(newUser);

        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var command = new ChangeBalanceCommand(
            balanceLimitProvider.Object, newUser.Id, delta);

        await command.ExecuteAsync(context, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var user = context.BalanceHistories.FirstOrDefault(u => u.UserId == newUser.Id);

        user.ShouldNotBeNull();
        user.BalanceBefore.ShouldBe(balanceBefore);
        user.BalanceAfter.ShouldBe(expectedBalance);
    }

    [Fact]
    public async Task Command_Should_Save_UserBalanceWasChangedIntegrationEvent_Correctly()
    {
        // Arrange
        var delta = 100m;
        var balanceBefore = 30m;
        var balanceLimit = 200m;

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();
        balanceLimitProvider.Setup(x => x.GetBalanceLimit()).Returns(balanceLimit);

        using var context = CreateContext();

        var newUser = CreateUser();
        newUser.Balance = balanceBefore;
        context.Users.Add(newUser);

        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var command = new ChangeBalanceCommand(
            balanceLimitProvider.Object, newUser.Id, delta);

        await command.ExecuteAsync(context, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var integrationEvent = context.IntegrationEvents.FirstOrDefault();

        integrationEvent.ShouldNotBeNull();
        integrationEvent.EntityId.ShouldBe(newUser.Id);
        integrationEvent.EventType.ShouldBe(typeof(UserBalanceWasChangedIntegrationEvent).AssemblyQualifiedName);
        integrationEvent.EventMetadata.ShouldNotBeNullOrEmpty();
        integrationEvent.IsSent.ShouldBeFalse();
        integrationEvent.NumberOfSendingAttempts.ShouldBe(0);
    }

    private static User CreateUser()
        => new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Иван",
            LastName = "Иванов",
            MiddleName = "Иванович",
            BirthDate = new DateOnly(1985, 6, 6),
            BirthPlace = "Москва"
        };
}