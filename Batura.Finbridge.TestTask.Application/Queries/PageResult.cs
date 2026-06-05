namespace Batura.Finbridge.TestTask.Application.Queries;

/// <summary>
/// Результат постраничного поиска
/// </summary>
/// <typeparam name="T">Тип элементов</typeparam>
public class PageResult<T>
{
    /// <summary>
    /// Номер текущей страницы
    /// </summary>
    public uint CurrentPage { get; set; }

    /// <summary>
    /// Общее количество страниц
    /// </summary>
    public uint TotalPages { get; set; }

    /// <summary>
    /// Общее количество элементов во всех страницах
    /// </summary>
    public uint TotalElements { get; set; }

    /// <summary>
    /// Элементы на текущей странице
    /// </summary>
    public required IEnumerable<T> Result { get; set; }
}