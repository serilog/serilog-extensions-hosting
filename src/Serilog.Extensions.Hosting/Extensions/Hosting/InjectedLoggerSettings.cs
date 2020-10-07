using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Configuration;
using Serilog.Core;

namespace Serilog.Extensions.Hosting
{
    class InjectedLoggerSettings : ILoggerSettings
    {
        readonly IServiceProvider _services;

        public InjectedLoggerSettings(IServiceProvider services)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public void Configure(LoggerConfiguration loggerConfiguration)
        {
            var levelSwitch = _services.GetService<LoggingLevelSwitch>();
            if (levelSwitch != null)
                loggerConfiguration.MinimumLevel.ControlledBy(levelSwitch);
            
            foreach (var settings in _services.GetServices<ILoggerSettings>())
                loggerConfiguration.ReadFrom.Settings(settings);

            foreach (var policy in _services.GetServices<IDestructuringPolicy>())
                loggerConfiguration.Destructure.With(policy);

            foreach (var enricher in _services.GetServices<ILogEventEnricher>())
                loggerConfiguration.Enrich.With(enricher);
            
            foreach (var filter in _services.GetServices<ILogEventFilter>())
                loggerConfiguration.Filter.With(filter);
            
            foreach (var sink in _services.GetServices<ILogEventSink>())
                loggerConfiguration.WriteTo.Sink(sink);
        }
    }
}
