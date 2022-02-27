using System;
using System.Collections.Generic;
using Serilog.Events;

namespace Serilog.Extensions.Hosting
{
    /// <summary>
    /// A container that receives properties added to a diagnostic context.
    /// </summary>
    public sealed class DiagnosticContextCollector : IDisposable
    {
        readonly IDisposable _chainedDisposable;
        readonly object _propertiesLock = new object();
        Exception _exception;
        Dictionary<string, LogEventProperty> _properties = new Dictionary<string, LogEventProperty>();

        /// <summary>
        /// Construct a <see cref="DiagnosticContextCollector"/>.
        /// </summary>
        /// <param name="chainedDisposable">An object that will be disposed to signal completion/disposal of
        /// the collector.</param>
        public DiagnosticContextCollector(IDisposable chainedDisposable)
        {
            _chainedDisposable = chainedDisposable ?? throw new ArgumentNullException(nameof(chainedDisposable));
        }

        /// <summary>
        /// Add the property to the context.
        /// </summary>
        /// <param name="property">The property to add.</param>
        public void AddOrUpdate(LogEventProperty property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            lock (_propertiesLock)
            {
                if (_properties == null) return;
                _properties[property.Name] = property;
            }
        }

        /// <summary>
        /// Set the exception associated with the current diagnostic context.
        /// </summary>
        /// <example>
        /// Passing an exception to the diagnostic context is useful when unhandled exceptions are handled before reaching Serilog's
        /// RequestLoggingMiddleware. One example is using https://www.nuget.org/packages/Hellang.Middleware.ProblemDetails to transform
        /// exceptions to ProblemDetails responses.
        /// </example>
        /// <remarks>
        /// If an unhandled exception reaches Serilog's RequestLoggingMiddleware, then the unhandled exception takes precedence.<br/>
        /// If <c>null</c> is given, it clears any previously assigned exception.
        /// </remarks>
        /// <param name="exception">The exception to log.</param>
        public void SetException(Exception exception)
        {
            lock (_propertiesLock)
            {
                if (_properties == null) return;
                _exception = exception;
            }
        }

        /// <summary>
        /// Complete the context and retrieve the properties added to it, if any. This will
        /// stop collection and remove the collector from the original execution context and
        /// any of its children.
        /// </summary>
        /// <param name="properties">The collected properties, or null if no collection is active.</param>
        /// <returns>True if properties could be collected.</returns>
        /// <seealso cref="IDiagnosticContext.Set"/>
        [Obsolete("Replaced by TryComplete(out IEnumerable<LogEventProperty> properties, out Exception exception).")]
        public bool TryComplete(out IEnumerable<LogEventProperty> properties)
        {
            return TryComplete(out properties, out _);
        }

        /// <summary>
        /// Complete the context and retrieve the properties and exception added to it, if any. This will
        /// stop collection and remove the collector from the original execution context and
        /// any of its children.
        /// </summary>
        /// <param name="properties">The collected properties, or null if no collection is active.</param>
        /// <param name="exception">The collected exception, or null if none has been collected or if no collection is active.</param>
        /// <returns>True if properties could be collected.</returns>
        /// <seealso cref="IDiagnosticContext.Set"/>
        /// <seealso cref="Serilog.IDiagnosticContext.SetException"/>
        public bool TryComplete(out IEnumerable<LogEventProperty> properties, out Exception exception)
        {
            lock (_propertiesLock)
            {
                properties = _properties?.Values;
                exception = _exception;
                _properties = null;
                _exception = null;
                Dispose();
                return properties != null;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _chainedDisposable.Dispose();
        }
    }
}
