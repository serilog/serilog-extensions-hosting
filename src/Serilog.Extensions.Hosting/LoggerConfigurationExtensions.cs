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

#if !NO_RELOADABLE_LOGGER

using Microsoft.Extensions.Hosting;
using Serilog.Extensions.Hosting;
using System;

namespace Serilog
{
    /// <summary>
    /// Extends <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationExtensions
    {
        /// <summary>
        /// Create a <see cref="ReloadableLogger"/> for use during host bootstrapping. The
        /// <see cref="SerilogHostBuilderExtensions.UseSerilog(IHostBuilder, Action{HostBuilderContext, IServiceProvider, LoggerConfiguration}, bool, bool)"/>
        /// configuration overload will detect when <see cref="Log.Logger"/> is set to a <see cref="ReloadableLogger"/> instance, and
        /// reconfigure/freeze it so that <see cref="ILogger"/>s created during host bootstrapping continue to work once
        /// logger configuration (with access to host services) is completed.
        /// </summary>
        /// <param name="loggerConfiguration"></param>
        /// <returns></returns>
        public static ReloadableLogger CreateBootstrapLogger(this LoggerConfiguration loggerConfiguration)
        {
            return new ReloadableLogger(loggerConfiguration.CreateLogger());
        }
    }
}

#endif
