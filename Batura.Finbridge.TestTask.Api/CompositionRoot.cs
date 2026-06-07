using Batura.Finbridge.TestTask.Infrastructure.Logging;
using Batura.Finbridge.TestTask.Infrastructure.Messaging;
using Batura.Finbridge.TestTask.Infrastructure.Outbox;
using Batura.Finbridge.TestTask.Model;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Reflection.PortableExecutable;

namespace Batura.Finbridge.TestTask.Api;

/// <summary>
/// Корень сборки приложения
/// </summary>
public static class CompositionRoot
{
    /// <summary>
    /// Определяет зависимости для приложения
    /// </summary>
    public static void DefineDependencies(WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddControllers();

        appBuilder.Services.AddSwaggerGen(options =>
        {
            var xmlPath = Path.Combine(
                AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

                    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                });

        SetLoggers(appBuilder);

        SetDbServices(appBuilder);

        SetUpEventBus(appBuilder);

        appBuilder.Services.Configure<OutboxOptions>(
            appBuilder.Configuration.GetSection("Outbox"));
    }

    /// <summary>
    /// Настраивает сервисы для работы с базой данных
    /// </summary>
    /// <param name="appBuilder">Построитель приложения</param>
    private static void SetDbServices(WebApplicationBuilder appBuilder)
    {
        string connectionStr = appBuilder.Configuration.GetConnectionString("Database")
            ?? throw new InvalidOperationException("Не настроена строка подключения к БД");

        appBuilder.Services.AddDbContextFactory<UserDbContext>(options 
            => options.UseNpgsql(connectionStr));
    }

    /// <summary>
    /// Конфигурирует шину для публикации событий интеграции
    /// </summary>
    /// <param name="appBuilder">Построитель приложения</param>
    private static void SetUpEventBus(WebApplicationBuilder appBuilder)
    {
        appBuilder.Services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<KafkaOptions>>().Value!;

            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = options.BootstrapServers,
                MessageTimeoutMs = 5000,
                RequestTimeoutMs = 3000,
                SocketTimeoutMs = 3000,
                
                MessageSendMaxRetries = 0,

                ReconnectBackoffMs = 500,
                ReconnectBackoffMaxMs = 1000,

                AllowAutoCreateTopics = true, // Только для тестового проекта. В production не использовать
            };

            var producer = new ProducerBuilder<string, string>(producerConfig).Build();

            return new KafkaEventBus(producer, options.Topic);
        });
    }

    /// <summary>
    /// Добавляет логирование
    /// </summary>
    /// <param name="appBuilder">Построитель приложения</param>
    private static void SetLoggers(WebApplicationBuilder appBuilder)
    {
        const string consoleOutputTemplate = "[{Timestamp:u} {Level}] [{CorrelationId}] {Message}{NewLine}{Exception}";
        const string fileOutputTemplate = consoleOutputTemplate;
        const string outputLogFile = "logs/log-.txt";

        // Setting up Serilog
        appBuilder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .MinimumLevel.Information() // Default logging level
            .Enrich.WithCorrelationId()
            .Enrich.WithCorrelationIdHeader()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal) // Ignore Microsoft logs
            .MinimumLevel.Override("System", LogEventLevel.Fatal) // Ignore System logs
            .WriteTo.Console(outputTemplate: consoleOutputTemplate) // Console format
            .WriteTo.File(outputLogFile,
                rollingInterval: RollingInterval.Day,
                outputTemplate: fileOutputTemplate)); // File format

        appBuilder.Services.AddTransient<ICorrelationContext, CorrelationContext>();
    }
}