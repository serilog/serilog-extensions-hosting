using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Serilog.Extensions.Hosting.Tests
{
    public class SerilogServiceCollectionExtensionsTests
    {
        [Fact]
        public void ServicesAreRegisteredWhenCallingAddSerilog()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddSerilog();

            // Assert
            using var provider = collection.BuildServiceProvider();
            provider.GetRequiredService<ILoggerFactory>();
            provider.GetRequiredService<IDiagnosticContext>();
        }

        [Fact]
        public void ServicesAreRegisteredWhenCallingAddSerilogWithLogger()
        {
            // Arrange
            var collection = new ServiceCollection();
            ILogger logger = new LoggerConfiguration().CreateLogger();

            // Act
            collection.AddSerilog(logger);

            // Assert
            using var provider = collection.BuildServiceProvider();
            provider.GetRequiredService<ILogger>();
            provider.GetRequiredService<ILoggerFactory>();
            provider.GetRequiredService<IDiagnosticContext>();
        }

        [Fact]
        public void ServicesAreRegisteredWhenCallingAddSerilogWithConfigureDelegate()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddSerilog(_ => { });

            // Assert
            using var provider = collection.BuildServiceProvider();
            provider.GetRequiredService<ILogger>();
            provider.GetRequiredService<ILoggerFactory>();
            provider.GetRequiredService<IDiagnosticContext>();
        }
    }
}
