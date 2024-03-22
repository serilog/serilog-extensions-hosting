using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Serilog.Events;

namespace Serilog.Extensions.Hosting.Tests.Support;

static class Some
{
    static int _next;

    public static int Int32() => Interlocked.Increment(ref _next);

    public static string String(string tag = null) => $"s_{tag}{Int32()}";

    public static LogEventProperty LogEventProperty() => new LogEventProperty(String("name"), new ScalarValue(Int32()));

    public static ILogger Logger() => new LoggerConfiguration().CreateLogger();
}
