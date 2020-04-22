// Copyright 2019 Serilog Contributors
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;

namespace Serilog
{
    /// <summary>
    /// Extends <see cref="IServiceCollection"/> with Serilog configuration methods.
    /// </summary>
    public static class SerilogServiceCollectionExtensions
    {
        /// <summary>
        /// Configures the <see cref="ILoggerFactory"/> to use the <see cref="SerilogLoggerFactory"/>
        /// </summary>
        /// <param name="collection">The service collection to configure.</param>
        /// <param name="logger">The Serilog logger; if not supplied, the static <see cref="Serilog.Log"/> will be used.</param>
        /// <param name="dispose">When <c>true</c>, dispose <paramref name="logger"/> when the framework disposes the provider. If the
        /// logger is not specified but <paramref name="dispose"/> is <c>true</c>, the <see cref="Log.CloseAndFlush()"/> method will be
        /// called on the static <see cref="Log"/> class instead.</param>
        /// <param name="providers">A <see cref="LoggerProviderCollection"/> registered in the Serilog pipeline using the
        /// <c>WriteTo.Providers()</c> configuration method, enabling other <see cref="ILoggerProvider"/>s to receive events. By
        /// default, only Serilog sinks will receive events.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddSerilogLoggerFactory(
            this IServiceCollection collection, 
            ILogger logger, 
            bool dispose,
            LoggerProviderCollection providers = null)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

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

            return collection;
        }

        /// <summary>
        /// Adds required Serilog services to the <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="logger"></param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddSerilogServices(this IServiceCollection collection, ILogger logger)
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

            return collection;
        }
    }
}
