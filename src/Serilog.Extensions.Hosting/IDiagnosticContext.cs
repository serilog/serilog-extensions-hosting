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

namespace Serilog
{
    /// <summary>
    /// Collects diagnostic information for packaging into wide events.
    /// </summary>
    public interface IDiagnosticContext
    {
        /// <summary>
        /// Set the specified property on the current diagnostic context. The property will be collected
        /// and attached to the event emitted at the completion of the context.
        /// </summary>
        /// <param name="propertyName">The name of the property. Must be non-empty.</param>
        /// <param name="value">The property value.</param>
        /// <param name="destructureObjects">If true, the value will be serialized as structured
        /// data if possible; if false, the object will be recorded as a scalar or simple array.</param>
        void Set(string propertyName, object value, bool destructureObjects = false);

        /// <summary>
        /// Set the specified exception on the current diagnostic context.
        /// </summary>
        /// <remarks>
        /// This method is useful when unhandled exceptions do not reach <c>Serilog.AspNetCore.RequestLoggingMiddleware</c>,
        /// such as when using <a href="https://www.nuget.org/packages/Hellang.Middleware.ProblemDetails">Hellang.Middleware.ProblemDetails</a>
        /// to transform exceptions to ProblemDetails responses.
        /// </remarks>
        /// <param name="exception">The exception to log. If <c>null</c> is given, it clears any previously assigned exception.</param>
        void SetException(Exception exception);
    }
}
