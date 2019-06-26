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
        readonly AmbientDiagnosticContextCollector _ambientCollector;
        readonly object _propertiesLock = new object();
        List<LogEventProperty> _properties = new List<LogEventProperty>();

        internal DiagnosticContextCollector(AmbientDiagnosticContextCollector ambientCollector)
        {
            _ambientCollector = ambientCollector ?? throw new ArgumentNullException(nameof(ambientCollector));
        }

        /// <summary>
        /// Add the property to the context.
        /// </summary>
        /// <param name="property">The property to add.</param>
        public void Add(LogEventProperty property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));

            lock (_propertiesLock)
            {
                _properties?.Add(property);
            }
        }

        /// <summary>
        /// Complete the context and retrieve the properties added to it, if any. This will
        /// stop collection and remove the collector from the original execution context and
        /// any of its children.
        /// </summary>
        /// <param name="properties">The collected properties, or null if no collection is active.</param>
        /// <returns>True if properties could be collected.</returns>
        public bool TryComplete(out List<LogEventProperty> properties)
        {
            lock (_propertiesLock)
            {
                properties = _properties;
                _properties = null;
                Dispose();
                return properties != null;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _ambientCollector.Dispose();
        }
    }
}
