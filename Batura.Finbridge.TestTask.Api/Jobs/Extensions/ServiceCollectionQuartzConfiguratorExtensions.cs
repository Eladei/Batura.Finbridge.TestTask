using Quartz;

namespace Batura.Finbridge.TestTask.Api.Jobs.Extensions;

/// <summary>
/// Методы расширения для настройки Quartz
/// </summary>
public static class ServiceCollectionQuartzConfiguratorExtensions
{
    /// <summary>
    /// Добавляет новое задание с расписанием
    /// </summary>
    /// <typeparam name="T">Тип задания</typeparam>
    /// <param name="q">Конфигуратор Quartz</param>
    /// <param name="cron">CRON-выражение, определяющее расписание выполнения задания</param>
    public static void AddNewJob<T>(this IServiceCollectionQuartzConfigurator q, string cron)
        where T : IJob
    {
        var jobName = typeof(T).Name;
        var jobKey = new JobKey(jobName);

        q.AddJob<T>(opts => opts.WithIdentity(jobKey));

        q.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{jobName}-trigger")
            .WithCronSchedule(cron)
        );
    }
}