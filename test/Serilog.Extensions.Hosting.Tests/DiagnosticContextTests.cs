using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Extensions.Hosting.Tests.Support;
using Xunit;

// ReSharper disable PossibleNullReferenceException

namespace Serilog.Extensions.Hosting.Tests
{
    public class DiagnosticContextTests
    {
        [Fact]
        public void SetIsSafeWhenNoContextIsActive()
        {
            var dc = new DiagnosticContext(Some.Logger());
            dc.Set(Some.String("name"), Some.Int32());
        }

        [Fact]
        public async Task PropertiesAreCollectedInAnActiveContext()
        {
            var dc = new DiagnosticContext(Some.Logger());

            var collector = dc.BeginCollection();
            dc.Set(Some.String("first"), Some.Int32());
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            dc.Set(Some.String("second"), Some.Int32());

            Assert.True(collector.TryComplete(out var properties));
            Assert.Equal(2, properties.Count());

            Assert.False(collector.TryComplete(out _));

            collector.Dispose();

            dc.Set(Some.String("third"), Some.Int32());
            Assert.False(collector.TryComplete(out _));
        }

        [Fact]
        public void ExistingPropertiesCanBeUpdated()
        {
            var dc = new DiagnosticContext(Some.Logger());

            var collector = dc.BeginCollection();
            dc.Set("name", 10);
            dc.Set("name", 20);

            Assert.True(collector.TryComplete(out var properties));
            var prop = Assert.Single(properties);
            var scalar = Assert.IsType<ScalarValue>(prop.Value);
            Assert.Equal(20, scalar.Value);
        }
    }
}
