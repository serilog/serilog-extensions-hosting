using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Serilog.Extensions.Hosting.Tests;

public class SerilogHostBuilderExtensionsTests
{
    [Fact]
    public void ServicesAreRegisteredWhenCallingUseSerilog()
    {
        // Arrange
        var collection = new ServiceCollection();
        IHostBuilder builder = new FakeHostBuilder(collection);

        // Act
        builder.UseSerilog();

        // Assert
        IServiceProvider provider = collection.BuildServiceProvider();
        provider.GetRequiredService<ILoggerFactory>();
        provider.GetRequiredService<IDiagnosticContext>();
    }

    [Fact]
    public void ServicesAreRegisteredWhenCallingUseSerilogWithLogger()
    {
        // Arrange
        var collection = new ServiceCollection();
        IHostBuilder builder = new FakeHostBuilder(collection);
        ILogger logger = new LoggerConfiguration().CreateLogger();

        // Act
        builder.UseSerilog(logger);

        // Assert
        IServiceProvider provider = collection.BuildServiceProvider();
        provider.GetRequiredService<ILogger>();
        provider.GetRequiredService<ILoggerFactory>();
        provider.GetRequiredService<IDiagnosticContext>();
    }

    [Fact]
    public void ServicesAreRegisteredWhenCallingUseSerilogWithConfigureDelegate()
    {
        // Arrange
        var collection = new ServiceCollection();
        IHostBuilder builder = new FakeHostBuilder(collection);

        // Act
        builder.UseSerilog((_, _) => { });

        // Assert
        IServiceProvider provider = collection.BuildServiceProvider();
        provider.GetRequiredService<ILogger>();
        provider.GetRequiredService<ILoggerFactory>();
        provider.GetRequiredService<IDiagnosticContext>();
    }

    private class FakeHostBuilder : IHostBuilder
    {
        private readonly IServiceCollection _collection;

        public FakeHostBuilder(IServiceCollection collection) => _collection = collection;

        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            configureDelegate(null, _collection);
            return this;
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            throw new NotImplementedException();
        }

        public IHost Build()
        {
            throw new NotImplementedException();
        }

        public IDictionary<object, object> Properties { get; }
    }
}
