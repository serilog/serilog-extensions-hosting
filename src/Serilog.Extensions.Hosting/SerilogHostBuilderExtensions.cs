// Copyright 2020 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;

namespace Serilog
{
    /// <summary>
    /// Extends <see cref="IHostBuilder"/> with Serilog configuration methods.
    /// </summary>
    public static class SerilogHostBuilderExtensions
    {
        // Used internally to pass information through the container.
        class RegisteredLogger
        {
            public RegisteredLogger(ILogger logger)
            {
                Logger = logger;
            }
            
            public ILogger Logger { get; }
        }

        /// <summary>
        /// Sets Serilog as the logging provider.
        /// </summary>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="Serilog.Log"/> will be used.</param>
        /// <param name="dispose">When <c>true</c>, dispose <paramref name="logger"/> when the framework disposes the provider. If the
        /// logger is not specified but <paramref name="dispose"/> is <c>true</c>, the <see cref="Serilog.Log.CloseAndFlush()"/> method will be
        /// called on the static <see cref="Serilog.Log"/> class instead.</param>
        /// <param name="providers">A <see cref="LoggerProviderCollection"/> registered in the Serilog pipeline using the
        /// <c>WriteTo.Providers()</c> configuration method, enabling other <see cref="Microsoft.Extensions.Logging.ILoggerProvider"/>s to receive events. By
        /// default, only Serilog sinks will receive events.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseSerilog(
            this IHostBuilder builder, 
            ILogger logger = null, 
            bool dispose = false,
            LoggerProviderCollection providers = null)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ConfigureServices((_, collection) =>
            {
                if (providers != null)
                {
                    collection.AddSingleton<ILoggerFactory>(services =>
                    {
                        var factory = new SerilogLoggerFactory(logger, dispose, providers);

                        foreach (var provider in services.GetServices<ILoggerProvider>())
                            factory.AddProvider(provider);

                        return factory;
                    });
                }
                else
                {
                    collection.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(logger, dispose));
                }

                ConfigureServices(collection, logger);
            });

            return builder;
        }


        /// <summary>Sets Serilog as the logging provider.</summary>
        /// <remarks>
        /// A <see cref="HostBuilderContext"/> is supplied so that configuration and hosting information can be used.
        /// The logger will be shut down when application services are disposed.
        /// </remarks>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="configureLogger">The delegate for configuring the <see cref="Serilog.LoggerConfiguration" /> that will be used to construct a <see cref="Serilog.Core.Logger" />.</param>
        /// <param name="preserveStaticLogger">Indicates whether to preserve the value of <see cref="Serilog.Log.Logger"/>.</param>
        /// <param name="writeToProviders">By default, Serilog does not write events to <see cref="ILoggerProvider"/>s registered through
        /// the Microsoft.Extensions.Logging API. Normally, equivalent Serilog sinks are used in place of providers. Specify
        /// <c>true</c> to write events to all providers.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseSerilog(
            this IHostBuilder builder,
            Action<HostBuilderContext, LoggerConfiguration> configureLogger,
            bool preserveStaticLogger = false,
            bool writeToProviders = false)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configureLogger == null) throw new ArgumentNullException(nameof(configureLogger));
            return UseSerilog(
                builder,
                (hostBuilderContext, services, loggerConfiguration) =>
                    configureLogger(hostBuilderContext, loggerConfiguration),
                preserveStaticLogger: preserveStaticLogger,
                writeToProviders: writeToProviders);
        }

        /// <summary>Sets Serilog as the logging provider.</summary>
        /// <remarks>
        /// A <see cref="HostBuilderContext"/> is supplied so that configuration and hosting information can be used.
        /// The logger will be shut down when application services are disposed.
        /// </remarks>
        /// <param name="builder">The host builder to configure.</param>
        /// <param name="configureLogger">The delegate for configuring the <see cref="Serilog.LoggerConfiguration" /> that will be used to construct a <see cref="Serilog.Core.Logger" />.</param>
        /// <param name="preserveStaticLogger">Indicates whether to preserve the value of <see cref="Serilog.Log.Logger"/>.</param>
        /// <param name="writeToProviders">By default, Serilog does not write events to <see cref="ILoggerProvider"/>s registered through
        /// the Microsoft.Extensions.Logging API. Normally, equivalent Serilog sinks are used in place of providers. Specify
        /// <c>true</c> to write events to all providers.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseSerilog(
            this IHostBuilder builder,
            Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> configureLogger,
            bool preserveStaticLogger = false,
            bool writeToProviders = false)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (configureLogger == null) throw new ArgumentNullException(nameof(configureLogger));
            
            builder.ConfigureServices((context, collection) =>
            {
                LoggerProviderCollection loggerProviders = null;
                if (writeToProviders)
                {
                    loggerProviders = new LoggerProviderCollection();
                }
                
                collection.AddSingleton(services =>
                {
                    var loggerConfiguration = new LoggerConfiguration();

                    if (loggerProviders != null)
                        loggerConfiguration.WriteTo.Providers(loggerProviders);
            
                    configureLogger(context, services, loggerConfiguration);
                    var logger = loggerConfiguration.CreateLogger();

                    return new RegisteredLogger(logger);
                });

                collection.AddSingleton(services =>
                {
                    // How can we register the logger, here, but not have MEDI dispose it?
                    // Using the `NullEnricher` hack to prevent disposal.
                    var logger = services.GetRequiredService<RegisteredLogger>().Logger;
                    return logger.ForContext(new NullEnricher());
                });
                
                collection.AddSingleton<ILoggerFactory>(services =>
                {
                    var logger = services.GetRequiredService<RegisteredLogger>().Logger;
                    
                    ILogger registeredLogger = null;
                    if (preserveStaticLogger)
                    {
                        registeredLogger = logger;
                    }
                    else
                    {
                        // Passing a `null` logger to `SerilogLoggerFactory` results in disposal via
                        // `Log.CloseAndFlush()`, which additionally replaces the static logger with a no-op.
                        Log.Logger = logger;
                    }

                    var factory = new SerilogLoggerFactory(registeredLogger, true, loggerProviders);

                    if (writeToProviders)
                    {
                        foreach (var provider in services.GetServices<ILoggerProvider>())
                            factory.AddProvider(provider);
                    }

                    return factory;
                });
                
                // Null is passed here because we've already (lazily) registered `ILogger`
                ConfigureServices(collection, null);
            });
            
            return builder;
        }

        static void ConfigureServices(IServiceCollection collection, ILogger logger)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            if (logger != null)
            {
                // This won't (and shouldn't) take ownership of the logger. 
                collection.AddSingleton(logger);
            }

            // Registered to provide two services...
            var diagnosticContext = new DiagnosticContext(logger);

            // Consumed by e.g. middleware
            collection.AddSingleton(diagnosticContext);

            // Consumed by user code
            collection.AddSingleton<IDiagnosticContext>(diagnosticContext);
        }
    }
}
