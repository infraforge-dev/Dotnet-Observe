using DotnetObserve.Core.Abstractions;
using DotnetObserve.Core.Logging;
using DotnetObserve.Core.Models;
using DotnetObserve.Core.Storage;
using DotnetObserve.Middleware;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IStore<LogEntry>>(
    sp => new JsonFileStore<LogEntry>(Path.Combine(builder.Environment.ContentRootPath, "logs.json"))
);

builder.Services.AddSingleton<IObservabilityLogger, DefaultObservabilityLogger>();

var app = builder.Build();

app.UseMiddleware<ObservabilityMiddleware>();

app.MapGet("/", () => "Hello World!");

app.MapGet("/hello", () => "Hello from SampleApi!");

app.MapGet("/throw", context => throw new InvalidOperationException("Something broke"));

app.Run();