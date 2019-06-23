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

namespace Serilog.Extensions.Hosting
{
    /// <summary>
    /// Implements an ambient/async-local diagnostic context. Consumers should
    /// use <see cref="IDiagnosticContext"/> in preference to this concrete type.
    /// </summary>
    public class DiagnosticContext : IDiagnosticContext
    {
        readonly ILogger _log;

        /// <summary>
        /// Construct a <see cref="DiagnosticContext"/>.
        /// </summary>
        /// <param name="log">A logger for binding properties in the context, or <c>null</c> to use <see cref="Log.Logger"/>.</param>
        public DiagnosticContext(ILogger log)
        {
            _log = log;
        }

        /// <summary>
        /// Start collecting properties to associate with the current async context. This will replace
        /// the active collector, if any.
        /// </summary>
        /// <returns>A collector that will receive events added in the current async context.</returns>
        public DiagnosticContextCollector BeginCollection()
        {
            return AmbientDiagnosticContextCollector.Begin();
        }

        /// <inheritdoc cref="IDiagnosticContext.Add"/>
        public void Add(string propertyName, object value, bool destructureObjects = false)
        {
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

            var collector = AmbientDiagnosticContextCollector.Current;
            if (collector != null && 
                (_log ?? Log.Logger).BindProperty(propertyName, value, destructureObjects, out var property))
            {
                collector.Add(property);
            }
        }
    }
}
