// Copyright 2019 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
