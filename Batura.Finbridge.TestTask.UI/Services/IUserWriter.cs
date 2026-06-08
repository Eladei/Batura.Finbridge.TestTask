using Batura.Finbridge.TestTask.Api.Contracts;

namespace Batura.Finbridge.TestTask.UI.Services;

public interface IUserWriter
{
    Task<Guid> RegisterUser(RegisterUserRequest request);

    Task ChangeBalance(ChangeBalanceRequest request);

    Task ChangeBalances(ChangeBalancesRequest request);
}