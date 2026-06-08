using Batura.Finbridge.TestTask.Api.Contracts;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Batura.Finbridge.TestTask.UI.Services;

public class UserReader : IUserReader
{
    private readonly BackendOptions _backendOptions;

    public UserReader(IOptions<BackendOptions> options) 
    {
        _backendOptions = options.Value
            ?? throw new ArgumentNullException("Не заданы настройки для взаимодействия с backend");
    }

    public async Task<UsersResponse> GetUsers(UsersRequest request) 
    {
        var handler = new HttpClientHandler
        {
            UseProxy = false,
            Proxy = null
        };

        using var client = new HttpClient(handler);

        var url = $"{_backendOptions.Host}/users?page=1&usersPerPage=20";
        var response = await client.GetAsync(url);

        var content = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<UsersResponse>(content, options) ?? new UsersResponse();
    }

    public async Task<BalanceHistoryResponse> GetBalanceHistory(BalanceHistoryRequest request) 
    {
        var handler = new HttpClientHandler
        {
            UseProxy = false,
            Proxy = null
        };

        using var client = new HttpClient(handler);

        var url = $"{_backendOptions.Host}/balances/history?page=1&historyItemsPerPage=20";
        var response = await client.GetAsync(url);

        var content = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<BalanceHistoryResponse>(content, options) ?? new BalanceHistoryResponse();
    }
}
