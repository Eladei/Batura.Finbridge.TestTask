using Batura.Finbridge.TestTask.Api.Contracts;
using Batura.Finbridge.TestTask.Application;
using Batura.Finbridge.TestTask.Application.Commands.Users;
using Batura.Finbridge.TestTask.Application.Queries.Users;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Batura.Finbridge.TestTask.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с балансами
    /// </summary>
    [ApiController]
    [Route("balances")]
    public class BalancesController : ControllerBase
    {
        private readonly IOperationExecutor _operationExecutor;
        private readonly IBalanceLimitProvider _balanceLimitProvider;

        /// <summary>
        /// Создает объект класса <see cref="UsersController"/>
        /// </summary>
        /// <param name="operationExecutor">Исполнитель операций</param>
        /// <param name="balanceLimitProvider">Провайдер лимита баланса пользователя</param>
        /// <exception cref="ArgumentNullException"></exception>
        public BalancesController(
            IOperationExecutor operationExecutor, 
            IBalanceLimitProvider balanceLimitProvider)
        {
            _operationExecutor = operationExecutor
                ?? throw new ArgumentNullException(nameof(operationExecutor));

            _balanceLimitProvider = balanceLimitProvider
                ?? throw new ArgumentNullException(nameof(balanceLimitProvider));
        }

        /// <summary>
        /// Получить историю изменения балансов пользователей
        /// </summary>
        /// <param name="request">Запрос на получение истории изменения баланса пользователей</param>
        /// <param name="token">Токен отмены операции</param>
        /// <returns>История изменения балансов пользователей</returns>
        [HttpGet("history")]
        public async Task<BalanceHistoryResponse> History([FromQuery] BalanceHistoryRequest request, CancellationToken token)
        {
            var query = new BalanceHistoryQuery(request.HistoryItemsPerPage, request.Page);

            var data = await _operationExecutor.ExecuteAsync(query, token);

            return new BalanceHistoryResponse()
            {
                TotalPages = data.TotalPages,
                History = [.. data.Result.Select(u => new BalanceHistoryItem
                { 
                    Id = u.Id,
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    MiddleName = u.MiddleName,
                    BalanceBefore = u.BalanceBefore,
                    BalanceAfter = u.BalanceAfter,
                    ChangedAtUtc = u.ChangedAtUtc
                })]
            };
        }

        /// <summary>
        /// Изменить баланс пользователя
        /// </summary>
        /// <param name="request">Запрос на изменение баланса пользователя</param>
        /// <param name="token">Токен отмены операции</param>
        [HttpPost("change")]
        public async Task<IActionResult> Change(ChangeBalanceRequest request, CancellationToken token)
        {
            var command = new ChangeBalanceCommand(
                _balanceLimitProvider, request.UserId, request.Delta);

            await _operationExecutor.ExecuteAsync(command, token);

            return Ok();
        }

        /// <summary>
        /// Изменить баланс нескольких пользователей
        /// </summary>
        /// <param name="request">Запрос на изменение баланса нескольких пользователей</param>
        /// <param name="token">Токен отмены операции</param>
        [HttpPost("batch")]
        public async Task<IActionResult> Batch(ChangeBalancesRequest request, CancellationToken token)
        {
            var balanceUpdateInfo = request.Items.Select(x => new BalanceUpdateInfo 
            { 
                UserId = x.UserId,
                Delta = x.Delta,
            }).ToArray();

            var command = new ChangeBalancesCommand(
                _balanceLimitProvider, balanceUpdateInfo);

            await _operationExecutor.ExecuteAsync(command, token);

            return Ok();
        }
    }
}
