using Microsoft.EntityFrameworkCore;

using Moq;

/// <summary>
/// Базовый класс для модульных тестов Entity Framework.
/// Предоставляет настроенный макет DbContext для изолированного тестирования без реальной базы данных.
/// </summary>
/// <typeparam name="T">Тип DbContext</typeparam>
public abstract class EFUnitTestsBase<T> where T : DbContext
{
    /// <summary>
    /// Замоканный или настроенный экземпляр DbContext, используемый в тестах
    /// </summary>
    protected T _context;

    /// <summary>
    /// Инициализирует новый экземпляр базового класса модульных тестов
    /// и настраивает DbContext с использованием mock-объекта.
    /// </summary>
    public EFUnitTestsBase()
    {
        _context = SetUpDbContext(new Mock<T>());
    }

    /// <summary>
    /// Настраивает и создаёт экземпляр DbContext на основе mock-объекта
    /// </summary>
    /// <param name="contextMock">Mock-объект DbContext</param>
    /// <returns>Настроенный экземпляр DbContext для тестирования</returns>
    protected abstract T SetUpDbContext(Mock<T> contextMock);
}