using Batura.Finbridge.TestTask.Application.Queries;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Базовый постраничный запрос для работы с Entity Framework
/// </summary>
/// <typeparam name="T">Тип контекста базы данных</typeparam>
/// <typeparam name="R">Тип элемента результата</typeparam>
public abstract class EfPageQueryBase<T, R> : EfQueryBase<T, PageResult<R>> where T : DbContext
{
    private readonly uint _page;
    protected readonly uint? _elementsPerPage;

    /// <inheritdoc />
    public override async Task<PageResult<R>> ExecuteAsync(T context, CancellationToken cancellationToken = default)
    {
        var result = await PerformAsync(context, cancellationToken);

        var pagesAdditionalInfo = await GetPagesAdditionalInfo(context, cancellationToken);

        return new PageResult<R>
        {
            CurrentPage = _page,
            TotalPages = pagesAdditionalInfo.TotalPages,
            TotalElements = pagesAdditionalInfo.TotalElements,
            Result = result
        };
    }

    /// <summary>
    /// Выполняет запрос
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Результат запроса</returns>
    protected abstract Task<IEnumerable<R>> PerformAsync(T context, CancellationToken cancellationToken);

    /// <summary>
    /// Получает общее количество элементов
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Общее количество элементов</returns>
    protected abstract Task<uint> GetAllElementsCount(T context, CancellationToken cancellationToken);

    /// <summary>
    /// Количество элементов, которые нужно пропустить
    /// </summary>
    protected uint ElementsToSkip => _elementsPerPage.HasValue
        ? _elementsPerPage.Value * (_page - 1)
        : 0;

    /// <summary>
    /// Создаёт экземпляр запроса
    /// </summary>
    /// <param name="elementsPerPage">Количество элементов на страницу</param>
    /// <param name="page">Номер страницы</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected EfPageQueryBase(uint? elementsPerPage = null, uint? page = null)
    {
        if (elementsPerPage.HasValue)
            ArgumentOutOfRangeException.ThrowIfZero(elementsPerPage.Value);

        if (page.HasValue)
            ArgumentOutOfRangeException.ThrowIfZero(page.Value);

        _elementsPerPage = elementsPerPage;
        _page = page ?? 1;
    }

    private async Task<PageAdditionalInfo> GetPagesAdditionalInfo(T context, CancellationToken cancellationToken)
    {
        var allElementsCount = await GetAllElementsCount(context, cancellationToken);

        var totalPages = _elementsPerPage.HasValue
            ? (uint)Math.Ceiling((double)allElementsCount / _elementsPerPage.Value)
            : allElementsCount;

        return new PageAdditionalInfo
        {
            TotalPages = totalPages,
            TotalElements = allElementsCount
        };
    }

    /// <summary>
    /// Дополнительная информация о пагинации
    /// </summary>
    private record PageAdditionalInfo
    {
        /// <summary>
        /// Общее количество страниц
        /// </summary>
        public uint TotalPages { get; init; }

        /// <summary>
        /// Общее количество элементов
        /// </summary>
        public uint TotalElements { get; init; }
    }
}