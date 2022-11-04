using CorrelationId;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICorrelationContext, CorrelationContext>();
builder.Services.AddScoped<IEventBus, EventBus>();
builder.Services.AddSingleton<SerilogCorrelationIdEnricher>();

builder.Host.UseSerilog((_, serviceProvider, config) =>
{
    config = config.MinimumLevel.Information();
    config.Enrich.FromLogContext()
        .Enrich.With(serviceProvider.GetRequiredService<SerilogCorrelationIdEnricher>())
        .WriteTo.Console(outputTemplate: "[{Timestamp:G} {Level:u3}] {Message:lj} Log Properties: {Properties}{NewLine}{Exception}{NewLine}{NewLine}",
            theme: AnsiConsoleTheme.Literate);
});

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseRouting();

app.MapGet("/ok", ([FromServices] IEventBus eventBus) =>
{
    eventBus.SendEvent();
    return Results.Ok("ok");
});

app.Run();