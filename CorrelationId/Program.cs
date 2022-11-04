using CorrelationId;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICorrelationContext, CorrelationContext>();
builder.Services.AddScoped<IEventBus, EventBus>();
builder.Services.AddSingleton<SerilogCorrelationIdEnricher>();

builder.Services.AddMediatR(typeof(Program));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CorrelationIdPipelineBehavior<,>));


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

app.MapGet("/ok", async ([FromServices] IEventBus eventBus,
    [FromServices] IMediator mediator) =>
{
    eventBus.SendEvent();
    var user = await mediator.Send(new GetUserQuery(userId: 2));

    return Results.Ok(user);
});

app.Run();