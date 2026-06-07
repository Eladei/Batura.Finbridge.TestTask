using Batura.Finbridge.TestTask.Application.Queries.Users;
using Batura.Finbridge.TestTask.Model;
using Batura.Finbridge.TestTask.Model.Entities;
using Shouldly;

namespace Batura.Finbridge.TestTask.Application.IntegrationTests.Queries;

/// <summary>
/// Интеграционные тесты для BalanceHistoryQuery
/// </summary>
/// <see cref="BalanceHistoryQuery"/>
public sealed class BalanceHistoryQueryTests : NpgsqlIntegrationTestsBase<UserDbContext>
{
    public BalanceHistoryQueryTests(NpgsqlConnectionParams serverConnectionParams)
        : base(serverConnectionParams, opts => new UserDbContext(opts)) { }

    [Fact]
    public async Task Query_Should_Return_Correct_Result()
    {
        // Arrange
        uint itemsPerPage = 20;
        uint pageNumber = 1;

        var now = DateTime.UtcNow;

        var idUser1 = Guid.NewGuid();
        var user1 = new User
        {
            Id = idUser1,
            FirstName = "Иван",
            LastName = "Иванов",
            MiddleName = "Иванович",
            BirthDate = new DateOnly(1985, 6, 6),
            BirthPlace = "Москва",
            Balance = 100m,
            CreatedAtUtc = now.AddMinutes(-120)
        };

        var idUser2 = Guid.NewGuid();
        var user2 = new User
        {
            Id = idUser2,
            FirstName = "Виктор",
            LastName = "Викторов",
            MiddleName = "Викторович",
            BirthDate = new DateOnly(1972, 1, 2),
            BirthPlace = "Санкт-Петербург",
            Balance = 200m,
            CreatedAtUtc = now.AddMinutes(-210)
        };

        var balanceHistories = new List<BalanceHistory>
        {
            new() 
            {
                Id = Guid.NewGuid(),
                UserId = idUser1,
                BalanceBefore = 0m,
                BalanceAfter = 35m,
                CreatedAtUtc = now.AddMinutes(-110),
                ChangedAtUtc = now.AddMinutes(-110)
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = idUser1,
                BalanceBefore = 35m,
                BalanceAfter = 100m,
                CreatedAtUtc = now.AddMinutes(-80),
                ChangedAtUtc = now.AddMinutes(-80)
            },
            new BalanceHistory
            {
                Id = Guid.NewGuid(),
                UserId = idUser2,
                BalanceBefore = 0m,
                BalanceAfter = 200m,
                CreatedAtUtc = now.AddMinutes(-90),
                ChangedAtUtc = now.AddMinutes(-90)
            }
        };

        user1.BalanceHistories.Add(balanceHistories[0]);
        user1.BalanceHistories.Add(balanceHistories[1]);
        user2.BalanceHistories.Add(balanceHistories[2]);

        using var context = CreateContext();

        context.Users.AddRange(user1, user2);

        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var query = new BalanceHistoryQuery(itemsPerPage, pageNumber);

        var result = await query.ExecuteAsync(context, CancellationToken.None);
        
        var foundBalanceHistory = result.Result.ToArray();

        // Assert
        foundBalanceHistory.Length.ShouldBe(3);

        ValidateUserInfo(foundBalanceHistory[0], balanceHistories[1], user1);
        ValidateUserInfo(foundBalanceHistory[1], balanceHistories[2], user2);
        ValidateUserInfo(foundBalanceHistory[2], balanceHistories[0], user1);
    }

    private static void ValidateUserInfo(
        BalanceHistoryItemReadModel checkingBalanceHistory, 
        BalanceHistory balanceHistory, User user)
    {
        checkingBalanceHistory.Id.ShouldBe(balanceHistory.Id);
        checkingBalanceHistory.UserId.ShouldBe(balanceHistory.UserId);
        checkingBalanceHistory.FirstName.ShouldBe(user.FirstName);
        checkingBalanceHistory.LastName.ShouldBe(user.LastName);
        checkingBalanceHistory.MiddleName.ShouldBe(user.MiddleName);
        checkingBalanceHistory.BalanceBefore.ShouldBe(balanceHistory.BalanceBefore);
        checkingBalanceHistory.BalanceAfter.ShouldBe(balanceHistory.BalanceAfter);
    }
}