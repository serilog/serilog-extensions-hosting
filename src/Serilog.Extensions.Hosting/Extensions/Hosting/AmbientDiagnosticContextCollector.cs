using System;
using System.Threading;

namespace Serilog.Extensions.Hosting
{
    class AmbientDiagnosticContextCollector : IDisposable
    {
        static readonly AsyncLocal<AmbientDiagnosticContextCollector> AmbientCollector =
            new AsyncLocal<AmbientDiagnosticContextCollector>();

        // The indirection here ensures that completing collection cleans up the collector in all
        // execution contexts. Via @benaadams' addition to `HttpContextAccessor` :-)
        DiagnosticContextCollector _collector;

        public static DiagnosticContextCollector Current => AmbientCollector.Value?._collector;

        public static DiagnosticContextCollector Begin()
        {
            var value = new AmbientDiagnosticContextCollector();
            value._collector = new DiagnosticContextCollector(value);
            AmbientCollector.Value = value;
            return value._collector;
        }

        public void Dispose()
        {
            _collector = null;
            if (AmbientCollector.Value == this)
                AmbientCollector.Value = null;
        }
    }
}
