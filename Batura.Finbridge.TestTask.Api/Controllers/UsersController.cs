using Batura.Finbridge.TestTask.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Batura.Finbridge.TestTask.Api.Controllers
{
    /// <summary>
    /// Контроллер для работы с пользователями
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        public UsersController()
        {
        }

        /// <summary>
        /// Получить зарегистрированных пользователей
        /// </summary>
        /// <returns>Зарегистрированные пользователи</returns>
        [HttpGet(Name = "Users")]
        public IEnumerable<User> GetUsers(CancellationToken token)
        {
            return
            [
                new() 
                { 
                    Id = Guid.NewGuid(),
                    FirstName = "Иван",
                    LastName = "Иванов",
                    MiddleName = "Иванович",
                    BirthDate = new DateOnly(1990, 1, 1),
                    BirthPlace = "Москва"
                }
            ];
        }

        /// <summary>
        /// Зарегистрировать нового пользователя
        /// </summary>
        /// <param name="request">Запрос на регистрацию пользователя</param>
        /// <param name="token">Токен отмены операции</param>
        [HttpPost(Name = "Users")]
        public IActionResult PostUsers(RegisterUserRequest request, CancellationToken token)
        {
            return Ok();
        }
    }
}
