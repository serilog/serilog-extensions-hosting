#if !NO_RELOADABLE_LOGGER

using System.Collections.Generic;
using System.Linq;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Hosting.Tests.Support;
using Xunit;

namespace Serilog.Extensions.Hosting.Tests
{
    public class ReloadableLoggerTests
    {
        [Fact]
        public void AFrozenLoggerYieldsSerilogLoggers()
        {
            var logger = new ReloadableLogger(new LoggerConfiguration().CreateLogger());
            var contextual = logger.ForContext<ReloadableLoggerTests>();

            var nested = contextual.ForContext("test", "test");
            Assert.IsNotType<Core.Logger>(nested);

            logger.Freeze();

            nested = contextual.ForContext("test", "test");
            Assert.IsType<Core.Logger>(nested);
        }
        
        [Fact]
        public void CachingReloadableLoggerRemainsUsableAfterFreezing()
        {
            var logger = new LoggerConfiguration().CreateBootstrapLogger();
            var contextual = logger.ForContext<ReloadableLoggerTests>();
            contextual.Information("First");
            logger.Reload(c => c);
            logger.Freeze();
            contextual.Information("Second");
            contextual.Information("Third");
            contextual.Information("Fourth"); // No crash :-)
        }

        [Fact]
        public void ReloadableLoggerRespectsMinimumLevelOverrides()
        {
            var emittedEvents = new List<LogEvent>();
            var logger = new LoggerConfiguration()
                .MinimumLevel.Override("Test", LogEventLevel.Warning)
                .WriteTo.Sink(new ListSink(emittedEvents))
                .CreateBootstrapLogger();

            var limited = logger
                .ForContext("X", 1)
                .ForContext(Constants.SourceContextPropertyName, "Test.Stuff");
            
            var notLimited = logger.ForContext<ReloadableLoggerTests>();
            
            foreach (var context in new[] { limited, notLimited })
            {
                // Suppressed by both sinks
                context.Debug("First");

                // Suppressed by the limited logger
                context.Information("Second");
                
                // Emitted by both loggers
                context.Warning("Third");
            }
            
            Assert.Equal(3, emittedEvents.Count);
            Assert.Equal(2, emittedEvents.Count(le => le.Level == LogEventLevel.Warning));
        }

        [Fact]
        public void ReloadableLoggersRecordEnrichment()
        {
            var emittedEvents = new List<LogEvent>();
            
            var logger = new LoggerConfiguration()
                .WriteTo.Sink(new ListSink(emittedEvents))
                .CreateBootstrapLogger();

            var outer = logger
                .ForContext("A", new object());
            var inner = outer.ForContext("B", "test");
            
            inner.Information("First");
            
            logger.Reload(lc => lc.WriteTo.Sink(new ListSink(emittedEvents)));
            
            inner.Information("Second");

            logger.Freeze();
            
            inner.Information("Third");
            
            outer.ForContext("B", "test").Information("Fourth");
            
            logger.ForContext("A", new object())
                .ForContext("B", "test")
                .Information("Fifth");
            
            Assert.Equal(5, emittedEvents.Count);
            Assert.All(emittedEvents, e => Assert.Equal(2, e.Properties.Count));
        }
    }
}

#endif
