// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Extensions.Hosting.Tests.Support
{
    public class ListSink : ILogEventSink
    {
        readonly List<LogEvent> _list;

        public ListSink(List<LogEvent> list)
        {
            _list = list;
        }

        public void Emit(LogEvent logEvent)
        {
            _list.Add(logEvent);
        }
    }
}