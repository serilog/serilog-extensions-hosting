#if !NO_RELOADABLE_LOGGER

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
    }
}

#endif
