using System;
using System.Threading.Tasks;
using Serilog.Extensions.Hosting.Tests.Support;
using Xunit;

namespace Serilog.Extensions.Hosting.Tests
{
    public class DiagnosticContextTests
    {
        [Fact]
        public void AddIsSafeWhenNoContextIsActive()
        {
            var dc = new DiagnosticContext(Some.Logger());
            dc.Add(Some.String("name"), Some.Int32());
        }

        [Fact]
        public async Task PropertiesAreCollectedInAnActiveContext()
        {
            var dc = new DiagnosticContext(Some.Logger());

            var collector = dc.BeginCollection();
            dc.Add(Some.String("name"), Some.Int32());
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            dc.Add(Some.String("name"), Some.Int32());

            Assert.True(collector.TryComplete(out var properties));
            Assert.Equal(2, properties.Count);

            Assert.False(collector.TryComplete(out _));

            collector.Dispose();

            dc.Add(Some.String("name"), Some.Int32());
            Assert.False(collector.TryComplete(out _));
        }
    }
}
