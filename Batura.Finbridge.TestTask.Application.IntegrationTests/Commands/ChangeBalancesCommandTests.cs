using Batura.Finbridge.TestTask.Application.Commands.Users;
using Batura.Finbridge.TestTask.Application.Exceptions;
using Batura.Finbridge.TestTask.Application.IntegrationEvents;
using Batura.Finbridge.TestTask.Application.Properties;
using Batura.Finbridge.TestTask.Model;
using Batura.Finbridge.TestTask.Model.Entities.Outbox;
using Moq;
using Shouldly;
using User = Batura.Finbridge.TestTask.Model.Entities.User;

namespace Batura.Finbridge.TestTask.Application.IntegrationTests.Commands;

/// <summary>
/// Интеграционные тесты для ChangeBalancesCommand
/// </summary>
/// <see cref="ChangeBalancesCommand"/>
public sealed class ChangeBalancesCommandTests : NpgsqlIntegrationTestsBase<UserDbContext>
{
    public ChangeBalancesCommandTests(NpgsqlConnectionParams serverConnectionParams)
        : base(serverConnectionParams, opts => new UserDbContext(opts)) { }

    [Fact]
    public async Task Command_Should_Throw_OperationLogicException_When_Users_Not_Found_By_Id()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var delta = 100m;
        var expectedError = string.Format(Resources.UsersNotFoundByIds, userId);

        var updateInfos = new[]
        {
            new BalanceUpdateInfo
            {
                UserId = userId,
                Delta = delta
            }
        };

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();

        using var context = CreateContext();

        // Act, Assert
        var command = new ChangeBalancesCommand(balanceLimitProvider.Object, updateInfos);

        var exception = await Assert.ThrowsAsync<OperationLogicException>(
            () => command.ExecuteAsync(context, CancellationToken.None));

        exception.Message.ShouldBe(expectedError);
    }

    [Fact]
    public async Task Command_Should_Throw_OperationLogicException_When_New_Balance_Negative()
    {
        // Arrange
        var deltaUser1 = -100m;
        var balanceBeforeUser1 = 30m;
        var expectedBalanceUser1 = deltaUser1 + balanceBeforeUser1;

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();

        using var context = CreateContext();

        var user1 = CreateUser(Guid.NewGuid(), "Иван", balanceBeforeUser1);
        context.Users.Add(user1);

        await context.SaveChangesAsync(CancellationToken.None);

        var expectedError = string.Format(
            Resources.BalanceCannotBeNegative, user1.Id, expectedBalanceUser1);

        var updateInfos = new[]
        {
            new BalanceUpdateInfo
            {
                UserId = user1.Id,
                Delta = deltaUser1
            }
        };

        // Act
        var command = new ChangeBalancesCommand(balanceLimitProvider.Object, updateInfos);

        // Assert
        var exception = await Assert.ThrowsAsync<OperationLogicException>(
            () => command.ExecuteAsync(context, CancellationToken.None));

        exception.ShouldNotBeNull();
        exception.Message.ShouldContain(user1.Id.ToString());
    }

    [Fact]
    public async Task Command_Should_Throw_OperationLogicException_When_Balance_Exceeds_Limit()
    {
        // Arrange
        var deltaUser1 = 80m;
        var balanceBeforeUser1 = 30m;
        var expectedBalanceUser1 = deltaUser1 + balanceBeforeUser1;

        var balanceLimit = 50m;

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();
        balanceLimitProvider.Setup(x => x.GetBalanceLimit()).Returns(balanceLimit);

        using var context = CreateContext();

        var user1 = CreateUser(Guid.NewGuid(), "Иван", balanceBeforeUser1);
        context.Users.Add(user1);

        await context.SaveChangesAsync(CancellationToken.None);

        var expectedError = string.Format(
            Resources.BalanceCannotExceedLimit, user1.Id, expectedBalanceUser1, balanceLimit);

        var updateInfos = new[]
        {
            new BalanceUpdateInfo
            {
                UserId = user1.Id,
                Delta = deltaUser1
            }
        };

        // Act
        var command = new ChangeBalancesCommand(balanceLimitProvider.Object, updateInfos);

        // Assert
        var exception = await Assert.ThrowsAsync<OperationLogicException>(
            () => command.ExecuteAsync(context, CancellationToken.None));

        exception.ShouldNotBeNull();
        exception.Message.ShouldContain(user1.Id.ToString());
    }

    [Fact]
    public async Task Command_Should_Change_Balance_Correctly()
    {
        // Arrange
        var balanceLimit = 200m;

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();
        balanceLimitProvider.Setup(x => x.GetBalanceLimit()).Returns(balanceLimit);

        var idUser1 = Guid.NewGuid();
        var deltaUser1 = 50m;
        var balanceBeforeUser1 = 100m;
        var expectedBalanceUser1 = 150m;

        var idUser2 = Guid.NewGuid();
        var deltaUser2 = 70m;
        var balanceBeforeUser2 = 60m;
        var expectedBalanceUser2 = 130m;

        using var context = CreateContext();

        var user1 = CreateUser(idUser1, "Виктор", balanceBeforeUser1);
        var user2 = CreateUser(idUser2, "Иван", balanceBeforeUser2);
        context.Users.AddRange(user1, user2);

        await context.SaveChangesAsync(CancellationToken.None);

        var updateInfos = new[]
        {
            new BalanceUpdateInfo
            {
                UserId = user1.Id,
                Delta = deltaUser1
            },
            new BalanceUpdateInfo 
            {
                UserId = user2.Id,
                Delta = deltaUser2
            }
        };

        // Act
        var command = new ChangeBalancesCommand(balanceLimitProvider.Object, updateInfos);

        await command.ExecuteAsync(context, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var users = context.Users.ToDictionary(u => u.Id);

        users.Keys.Count.ShouldBe(2);

        users[idUser1].Balance.ShouldBe(expectedBalanceUser1);
        users[idUser2].Balance.ShouldBe(expectedBalanceUser2);
    }

    [Fact]
    public async Task Command_Should_Change_Save_All_UserBalanceWasChangedIntegrationEvent_Correctly()
    {
        // Arrange
        var balanceLimit = 200m;

        var balanceLimitProvider = new Mock<IBalanceLimitProvider>();
        balanceLimitProvider.Setup(x => x.GetBalanceLimit()).Returns(balanceLimit);

        var idUser1 = Guid.NewGuid();
        var deltaUser1 = 50m;
        var balanceBeforeUser1 = 100m;

        var idUser2 = Guid.NewGuid();
        var deltaUser2 = 70m;
        var balanceBeforeUser2 = 60m;

        using var context = CreateContext();

        var user1 = CreateUser(idUser1, "Виктор", balanceBeforeUser1);
        var user2 = CreateUser(idUser2, "Иван", balanceBeforeUser2);
        context.Users.AddRange(user1, user2);

        await context.SaveChangesAsync(CancellationToken.None);

        var updateInfos = new[]
        {
            new BalanceUpdateInfo
            {
                UserId = user1.Id,
                Delta = deltaUser1
            },
            new BalanceUpdateInfo
            {
                UserId = user2.Id,
                Delta = deltaUser2
            }
        };

        // Act
        var command = new ChangeBalancesCommand(balanceLimitProvider.Object, updateInfos);

        await command.ExecuteAsync(context, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        // Assert
        var events = context.IntegrationEvents.ToDictionary(x => x.EntityId);
        events.Keys.Count.ShouldBe(2);

        ValidateIntegrationEvent(events[idUser1], idUser1);
        ValidateIntegrationEvent(events[idUser2], idUser2);
    }

    private static void ValidateIntegrationEvent(IntegrationEventToSend integrationEvent, Guid userId)
    {
        integrationEvent.ShouldNotBeNull();
        integrationEvent.EntityId.ShouldBe(userId);
        integrationEvent.EventType.ShouldBe(typeof(UserBalanceWasChangedIntegrationEvent).AssemblyQualifiedName);
        integrationEvent.EventMetadata.ShouldNotBeNullOrEmpty();
        integrationEvent.IsSent.ShouldBeFalse();
        integrationEvent.NumberOfSendingAttempts.ShouldBe(0);
    }

    private static User CreateUser(Guid userId, string firstName, decimal balance)
        => new User
        {
            Id = userId,
            FirstName = firstName,
            LastName = "Иванов",
            MiddleName = "Иванович",
            BirthDate = new DateOnly(1985, 6, 6),
            BirthPlace = "Москва",
            Balance = balance
        };
}