using Batura.Finbridge.TestTask.Api.Contracts;

namespace Batura.Finbridge.TestTask.UI.Services;

public interface IUserReader
{
    Task<UsersResponse> GetUsers(UsersRequest request);

    Task<BalanceHistoryResponse> GetBalanceHistory(BalanceHistoryRequest request);
}
