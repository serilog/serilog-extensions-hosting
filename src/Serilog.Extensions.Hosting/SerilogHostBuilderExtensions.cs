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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
// ReSharper disable MemberCanBePrivate.Global

namespace Serilog
{
    /// <summary>
    /// Extends <see cref="IHostBuilder"/> with Serilog configuration methods.
    /// </summary>
    public static class SerilogHostBuilderExtensions
    {
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
                collection.AddSerilog(logger, dispose, providers);
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
        /// <remarks>If the static <see cref="Log.Logger"/> is a bootstrap logger (see
        /// <c>LoggerConfigurationExtensions.CreateBootstrapLogger()</c>), and <paramref name="preserveStaticLogger"/> is
        /// not specified, the the bootstrap logger will be reconfigured through the supplied delegate, rather than being
        /// replaced entirely or ignored.</remarks>
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
                collection.AddSerilog(
                    (services, loggerConfiguration) =>
                        configureLogger(context, services, loggerConfiguration),
                    preserveStaticLogger: preserveStaticLogger,
                    writeToProviders: writeToProviders);
            });
            
            return builder;
        }
    }
}
