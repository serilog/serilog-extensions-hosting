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
        List<LogEventProperty> _properties = new List<LogEventProperty>();

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

                for (var i = 0; i < _properties.Count; ++i)
                {
                    if (_properties[i].Name == property.Name)
                    {
                        _properties[i] = property;
                        return;
                    }
                }

                _properties.Add(property);
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
            _chainedDisposable.Dispose();
        }
    }
}
