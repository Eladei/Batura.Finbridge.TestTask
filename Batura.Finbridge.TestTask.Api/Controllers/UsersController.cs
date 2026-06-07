using Batura.Finbridge.TestTask.Api.Contracts;
using Batura.Finbridge.TestTask.Application;
using Batura.Finbridge.TestTask.Application.Commands.Users;
using Batura.Finbridge.TestTask.Application.Queries.Users;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Batura.Finbridge.TestTask.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с пользователями
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IOperationExecutor _operationExecutor;

        /// <summary>
        /// Создает объект класса <see cref="UsersController"/>
        /// </summary>
        /// <param name="operationExecutor">Исполнитель операций</param>
        /// <exception cref="ArgumentNullException"></exception>
        public UsersController(IOperationExecutor operationExecutor)
        {
            _operationExecutor = operationExecutor
                ?? throw new ArgumentNullException(nameof(operationExecutor));
        }

        /// <summary>
        /// Получить зарегистрированных пользователей
        /// </summary>
        /// <param name="request">Запрос зарегистрированных пользователей</param>
        /// <param name="token">Токен отмены операции</param>
        /// <returns>Зарегистрированные пользователи</returns>
        [HttpGet(Name = "Users")]
        public async Task<UsersResponse> GetUsers([FromQuery] UsersRequest request, CancellationToken token)
        {
            var query = new UsersQuery(request.UsersPerPage, request.Page);

            var data = await _operationExecutor.ExecuteAsync(query, token);

            return new UsersResponse()
            {
                TotalPages = data.TotalPages,
                Users = [.. data.Result.Select(u => new User 
                { 
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    MiddleName = u.MiddleName,
                    BirthDate = u.BirthDate,
                    BirthPlace = u.BirthPlace,
                    Balance = u.Balance,
                    RegisteredAtUtc = u.RegisteredAtUtc
                })]
            };
        }

        /// <summary>
        /// Зарегистрировать нового пользователя
        /// </summary>
        /// <param name="request">Запрос на регистрацию пользователя</param>
        /// <param name="token">Токен отмены операции</param>
        [HttpPost(Name = "Users")]
        public async Task<IActionResult> PostUsers(RegisterUserRequest request, CancellationToken token)
        {
            var command = new RegisterUserCommand(
                request.FirstName, request.LastName, request.MiddleName,
                request.BirthDate, request.BirthPlace);

            await _operationExecutor.ExecuteAsync(command, token);

            return Ok();
        }
    }
}
