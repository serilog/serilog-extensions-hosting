using System;
using System.Linq;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Parsing;

namespace Serilog.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DiagnosticContextScope : IDisposable
    {
        readonly LogEventLevel _level;
        readonly string _messageTemplate;
        readonly object[] _properties;
        readonly DiagnosticContextCollector _collector;

        static readonly LogEventProperty[] NoProperties = new LogEventProperty[0];
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="diagnosticContext"></param>
        /// <param name="level"></param>
        /// <param name="messageTemplate"></param>
        /// <param name="properties"></param>
        public DiagnosticContextScope(DiagnosticContext diagnosticContext,
            LogEventLevel level,
            string messageTemplate,
            object[] properties)
        {
            _level = level;
            _messageTemplate = messageTemplate;
            _properties = properties;

            _collector = diagnosticContext.BeginCollection();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            var logger = Log.ForContext<DiagnosticContextScope>();

            if (!_collector.TryComplete(out var collectedProperties))
                collectedProperties = NoProperties;

            foreach(var collectedProp in collectedProperties) {
                logger = logger.ForContext(collectedProp.Name, collectedProp.Value);
            }
            
            logger.Write(_level, (Exception)null, _messageTemplate, _properties);
        }
    }
}