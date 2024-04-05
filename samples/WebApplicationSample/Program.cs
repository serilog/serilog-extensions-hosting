using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
        
    Log.Information("Starting up!");

try
{
    var builder = WebApplication.CreateBuilder();

    builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
        .WriteTo.Console()
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services));

    var app = builder.Build();

    app.MapGet("/", () =>
    {
        Log.Information("Saying hello");
        return "Hello World!";
    });
    
    await app.RunAsync();
    
    Log.Information("Stopped cleanly");
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}
