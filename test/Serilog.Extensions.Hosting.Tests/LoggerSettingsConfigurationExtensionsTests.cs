using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Hosting.Tests.Support;
using Xunit;

namespace Serilog.Extensions.Hosting.Tests;

public class LoggerSettingsConfigurationExtensionsTests
{
    [Fact]
    public void SinksAreInjectedFromTheServiceProvider()
    {
        var emittedEvents = new List<LogEvent>();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ILogEventSink>(new ListSink(emittedEvents));
        using var services = serviceCollection.BuildServiceProvider();

        using var logger = new LoggerConfiguration()
            .ReadFrom.Services(services)
            .CreateLogger();

        logger.Information("Hello, world!");

        var evt = Assert.Single(emittedEvents);
        Assert.Equal("Hello, world!", evt!.MessageTemplate.Text);
    }
}