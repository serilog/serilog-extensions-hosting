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
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Extensions.Hosting;

namespace Serilog
{
    /// <summary>
    /// Extends <see cref="LoggerSettingsConfiguration"/> with methods for consuming host services.
    /// </summary>
    public static class LoggerSettingsConfigurationExtensions
    {
        /// <summary>
        /// Configure the logger using components from the <paramref name="services"/>. If present, the logger will
        /// receive implementations/instances of <see cref="LoggingLevelSwitch"/>, <see cref="IDestructuringPolicy"/>,
        /// <see cref="ILogEventFilter"/>, <see cref="ILogEventEnricher"/>, <see cref="ILogEventSink"/>, and
        /// <see cref="ILoggerSettings"/>.
        /// </summary>
        /// <param name="loggerSettingsConfiguration">The `ReadFrom` configuration object.</param>
        /// <param name="services">A <see cref="IServiceProvider"/> from which services will be requested.</param>
        /// <returns>A <see cref="LoggerConfiguration"/> to support method chaining.</returns>
        public static LoggerConfiguration Services(
            this LoggerSettingsConfiguration loggerSettingsConfiguration,
            IServiceProvider services)
        {
            return loggerSettingsConfiguration.Settings(new InjectedLoggerSettings(services));
        }
    }
}