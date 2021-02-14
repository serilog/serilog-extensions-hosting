using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Serilog.Extensions.Hosting.Tests.Support;
using Xunit;

namespace Serilog.Extensions.Hosting.Tests
{
    public class LoggerSettingsConfigurationExtensionsTests
    {
        [Fact]
        public void SinksAreInjectedFromTheServiceProvider()
        {
            var sink = new SerilogSink();
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ILogEventSink>(sink);
            using var services = serviceCollection.BuildServiceProvider();

            using var logger = new LoggerConfiguration()
                .ReadFrom.Services(services)
                .CreateLogger();
            
            logger.Information("Hello, world!");
            
            var evt = Assert.Single(sink.Writes);
            Assert.Equal("Hello, world!", evt!.MessageTemplate.Text);
        }
    }
}