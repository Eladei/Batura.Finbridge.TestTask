using Batura.Finbridge.TestTask.Api.Middleware;

namespace Batura.Finbridge.TestTask.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        CompositionRoot.DefineDependencies(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ErrorMiddleware>();

        app.UseHttpsRedirection();

        app.MapControllers();

        app.Run();
    }
}