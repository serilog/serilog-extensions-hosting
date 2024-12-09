using Serilog.Events;
using Serilog.Extensions.Hosting.Tests.Support;
using Xunit;

// ReSharper disable PossibleNullReferenceException

namespace Serilog.Extensions.Hosting.Tests;

public class DiagnosticContextTests
{
    [Fact]
    public void SetIsSafeWhenNoContextIsActive()
    {
        var dc = new DiagnosticContext(Some.Logger());
        dc.Set(Some.String("name"), Some.Int32());
    }

    [Fact]
    public void SetExceptionIsSafeWhenNoContextIsActive()
    {
        var dc = new DiagnosticContext(Some.Logger());
        dc.SetException(new Exception("test"));
    }

    [Fact]
    public async Task PropertiesAreCollectedInAnActiveContext()
    {
        var dc = new DiagnosticContext(Some.Logger());

        var collector = dc.BeginCollection();
        dc.Set(Some.String("first"), Some.Int32());
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        dc.Set(Some.String("second"), Some.Int32());

        Assert.True(collector.TryComplete(out var properties, out var exception));
        Assert.Equal(2, properties!.Count());
        Assert.Null(exception);

        Assert.False(collector.TryComplete(out _, out _));

        collector.Dispose();

        dc.Set(Some.String("third"), Some.Int32());
        Assert.False(collector.TryComplete(out _, out _));
    }

    [Fact]
    public void ExceptionIsCollectedInAnActiveContext()
    {
        var dc = new DiagnosticContext(Some.Logger());
        var collector = dc.BeginCollection();

        var setException = new Exception("before collect");
        dc.SetException(setException);

        Assert.True(collector.TryComplete(out _, out var collectedException));
        Assert.Same(setException, collectedException);
    }

    [Fact]
    public void ExceptionIsNotCollectedAfterTryComplete()
    {
        var dc = new DiagnosticContext(Some.Logger());
        var collector = dc.BeginCollection();
        collector.TryComplete(out _, out _);
        dc.SetException(new Exception(Some.String("after collect")));

        var tryComplete2 = collector.TryComplete(out _, out var collectedException2);

        Assert.False(tryComplete2);
        Assert.Null(collectedException2);
    }

    [Fact]
    public void ExceptionIsNotCollectedAfterDispose()
    {
        var dc = new DiagnosticContext(Some.Logger());
        var collector = dc.BeginCollection();
        collector.Dispose();

        dc.SetException(new Exception("after dispose"));

        var tryComplete = collector.TryComplete(out _, out var collectedException);

        Assert.True(tryComplete);
        Assert.Null(collectedException);
    }

    [Fact]
    public void ExistingPropertiesCanBeUpdated()
    {
        var dc = new DiagnosticContext(Some.Logger());

        var collector = dc.BeginCollection();
        dc.Set("name", 10);
        dc.Set("name", 20);

        Assert.True(collector.TryComplete(out var properties, out var exception));
        var prop = Assert.Single(properties!);
        var scalar = Assert.IsType<ScalarValue>(prop.Value);
        Assert.Equal(20, scalar.Value);
        Assert.Null(exception);
    }

    [Fact]
    public void ExistingExceptionCanBeUpdated()
    {
        var dc = new DiagnosticContext(Some.Logger());
        var collector = dc.BeginCollection();

        dc.SetException(new Exception("ex1"));
        dc.SetException(new Exception("ex2"));

        Assert.True(collector.TryComplete(out _, out var collectedException));
        Assert.Equal("ex2", collectedException!.Message);
    }
}
