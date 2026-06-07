using Batura.Finbridge.TestTask.Application.Queries.Users;
using Batura.Finbridge.TestTask.Model;
using Batura.Finbridge.TestTask.Model.Entities;
using Shouldly;

namespace Batura.Finbridge.TestTask.Application.IntegrationTests.Queries;

/// <summary>
/// Интеграционные тесты для UsersQuery
/// </summary>
/// <see cref="UsersQuery"/>
public sealed class UsersQueryTests : NpgsqlIntegrationTestsBase<UserDbContext>
{
    public UsersQueryTests(NpgsqlConnectionParams serverConnectionParams)
        : base(serverConnectionParams, opts => new UserDbContext(opts)) { }

    [Fact]
    public async Task Query_Should_Return_Correct_Result()
    {
        // Arrange
        uint itemsPerPage = 20;
        uint pageNumber = 1;

        var user1 = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Иван",
            LastName = "Иванов",
            MiddleName = "Иванович",
            BirthDate = new DateOnly(1985, 6, 6),
            BirthPlace = "Москва",
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-20)
        };

        var user2 = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Виктор",
            LastName = "Викторов",
            MiddleName = "Викторович",
            BirthDate = new DateOnly(1972, 1, 2),
            BirthPlace = "Санкт-Петербург",
            CreatedAtUtc = DateTime.UtcNow.AddMinutes(-10)
        };

        using var context = CreateContext();

        context.Users.AddRange(user1, user2);

        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var query = new UsersQuery(itemsPerPage, pageNumber);

        // Act
        var result = await query.ExecuteAsync(context, CancellationToken.None);
        
        var usersInfo = result.Result.ToArray();

        // Assert
        usersInfo.Length.ShouldBe(2);

        ValidateUserInfo(usersInfo[0], user2);
        ValidateUserInfo(usersInfo[1], user1);
    }

    private static void ValidateUserInfo(UserReadModel userInfo, User user)
    {
        userInfo.Id.ShouldBe(user.Id);
        userInfo.FirstName.ShouldBe(user.FirstName);
        userInfo.LastName.ShouldBe(user.LastName);
        userInfo.MiddleName.ShouldBe(user.MiddleName);
        userInfo.BirthDate.ShouldBe(user.BirthDate);
        userInfo.BirthPlace.ShouldBe(user.BirthPlace);
    }
}