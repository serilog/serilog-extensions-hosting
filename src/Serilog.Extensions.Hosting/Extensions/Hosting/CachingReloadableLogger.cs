// Copyright 2020 Serilog Contributors
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

#if !NO_RELOADABLE_LOGGER

using System;
using System.Collections.Generic;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace Serilog.Extensions.Hosting
{
    class CachingReloadableLogger : ILogger, IReloadableLogger
    {
        readonly ReloadableLogger _reloadableLogger;
        readonly Func<ILogger, ILogger> _configure;
        readonly IReloadableLogger _parent;
        
        ILogger _root, _cached;
        bool _frozen;

        public CachingReloadableLogger(ReloadableLogger reloadableLogger, ILogger root, IReloadableLogger parent, Func<ILogger, ILogger> configure)
        {
            _reloadableLogger = reloadableLogger;
            _parent = parent;
            _configure = configure;
            _root = root;
            _cached = null;
            _frozen = false;
        }

        public ILogger ReloadLogger()
        {
            return _configure(_parent.ReloadLogger());
        }

        public ILogger ForContext(ILogEventEnricher enricher)
        {
            if (enricher == null) return this;
            
            if (_frozen)
                return _cached.ForContext(enricher);

            if (_reloadableLogger.CreateChild(
                _root,
                this,
                _cached, 
                p => p.ForContext(enricher),
                out var child,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }

            return child;
        }

        public ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
        {
            if (enrichers == null) return this;
            
            if (_frozen)
                return _cached.ForContext(enrichers);

            if (_reloadableLogger.CreateChild(
                _root,
                this,
                _cached, 
                p => p.ForContext(enrichers),
                out var child,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }

            return child;
        }

        public ILogger ForContext(string propertyName, object value, bool destructureObjects = false)
        {
            if (propertyName == null) return this;
            
            if (_frozen)
                return _cached.ForContext(propertyName, value, destructureObjects);

            ILogger child;
            if (value == null || value is string || value.GetType().IsPrimitive || value.GetType().IsEnum)
            {
                // Safe to extend the lifetime of `value` by closing over it.
                // This ensures `SourceContext` is passed through appropriately and triggers minimum level overrides.
                if (_reloadableLogger.CreateChild(
                    _root,
                    this,
                    _cached,
                    p => p.ForContext(propertyName, value, destructureObjects),
                    out child,
                    out var newRoot,
                    out var newCached,
                    out var frozen))
                {
                    Update(newRoot, newCached, frozen);
                }
            }
            else
            {
                // It's not safe to extend the lifetime of `value` or pass it unexpectedly between threads.
                // Changes to destructuring configuration won't be picked up by the cached logger.
                var eager = ReloadLogger();
                if (!eager.BindProperty(propertyName, value, destructureObjects, out var property))
                    return this;

                var enricher = new FixedPropertyEnricher(property);
            
                if (_reloadableLogger.CreateChild(
                    _root,
                    this,
                    _cached,
                    p => p.ForContext(enricher),
                    out child,
                    out var newRoot,
                    out var newCached,
                    out var frozen))
                {
                    Update(newRoot, newCached, frozen);
                }
            }

            return child;
        }

        public ILogger ForContext<TSource>()
        {
            if (_frozen)
                return _cached.ForContext<TSource>();
            
            if (_reloadableLogger.CreateChild(
                _root,
                this,
                _cached, 
                p => p.ForContext<TSource>(),
                out var child,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }

            return child;
        }

        public ILogger ForContext(Type source)
        {
            if (_frozen)
                return _cached.ForContext(source);

            if (_reloadableLogger.CreateChild(
                _root,
                this,
                _cached, 
                p => p.ForContext(source),
                out var child,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }

            return child;
        }

        void Update(ILogger newRoot, ILogger newCached, bool frozen)
        {
            _root = newRoot;
            _cached = newCached;
                
            // https://github.com/dotnet/runtime/issues/20500#issuecomment-284774431
            // Publish `_cached` and then `_frozen`. This is useful here because it means that once the logger is frozen - which
            // we always expect - reads don't require any synchronization/interlocked instructions.
            Interlocked.MemoryBarrierProcessWide();

            _frozen = frozen;

            Interlocked.MemoryBarrierProcessWide();
        }

        public void Write(LogEvent logEvent)
        {
            if (_frozen)
            {
                _cached.Write(logEvent);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                logEvent,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public void Write(LogEventLevel level, string messageTemplate)
        {
            if (_frozen)
            {
                _cached.Write(level, messageTemplate);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                level,
                messageTemplate,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue)
        {
            if (_frozen)
            {
                _cached.Write(level, messageTemplate, propertyValue);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                level,
                messageTemplate,
                propertyValue,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public void Write<T0, T1>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            if (_frozen)
            {
                _cached.Write(level, messageTemplate, propertyValue0, propertyValue1);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                level,
                messageTemplate,
                propertyValue0,
                propertyValue1,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public void Write<T0, T1, T2>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            if (_frozen)
            {
                _cached.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                level,
                messageTemplate,
                propertyValue0,
                propertyValue1,
                propertyValue2,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues)
        {
            if (_frozen)
            {
                _cached.Write(level, messageTemplate, propertyValues);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                level,
                messageTemplate,
                propertyValues,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public void Write(LogEventLevel level, Exception exception, string messageTemplate)
        {
            if (_frozen)
            {
                _cached.Write(level, exception, messageTemplate);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                level,
                exception,
                messageTemplate,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public void Write<T>(LogEventLevel level, Exception exception, string messageTemplate, T propertyValue)
        {
            if (_frozen)
            {
                _cached.Write(level, exception, messageTemplate, propertyValue);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                level,
                exception,
                messageTemplate,
                propertyValue,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public void Write<T0, T1>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0,
            T1 propertyValue1)
        {
            if (_frozen)
            {
                _cached.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                level,
                exception,
                messageTemplate,
                propertyValue0,
                propertyValue1,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public void Write<T0, T1, T2>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0,
            T1 propertyValue1, T2 propertyValue2)
        {
            if (_frozen)
            {
                _cached.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                level,
                exception,
                messageTemplate,
                propertyValue0,
                propertyValue1,
                propertyValue2,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (_frozen)
            {
                _cached.Write(level, exception, messageTemplate, propertyValues);
                return;
            }

            if (_reloadableLogger.InvokeWrite(
                _root,
                _cached,
                this,
                level,
                exception,
                messageTemplate,
                propertyValues,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }
        }

        public bool IsEnabled(LogEventLevel level)
        {
            if (_frozen)
            {
                return _cached.IsEnabled(level);
            }

            if (_reloadableLogger.InvokeIsEnabled(
                _root,
                _cached,
                this,
                level,
                out var isEnabled,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }

            return isEnabled;
        }
        
        public bool BindMessageTemplate(string messageTemplate, object[] propertyValues, out MessageTemplate parsedTemplate,
            out IEnumerable<LogEventProperty> boundProperties)
        {
            if (_frozen)
            {
                return _cached.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
            }

            if (_reloadableLogger.InvokeBindMessageTemplate(
                _root,
                _cached,
                this,
                messageTemplate,
                propertyValues,
                out parsedTemplate,
                out boundProperties,
                out var canBind,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }

            return canBind;
        }

        public bool BindProperty(string propertyName, object value, bool destructureObjects, out LogEventProperty property)
        {
            if (_frozen)
            {
                return _cached.BindProperty(propertyName, value, destructureObjects, out property);
            }

            if (_reloadableLogger.InvokeBindProperty(
                _root,
                _cached,
                this,
                propertyName,
                value,
                destructureObjects,
                out property,
                out var canBind,
                out var newRoot,
                out var newCached,
                out var frozen))
            {
                Update(newRoot, newCached, frozen);
            }

            return canBind;
        }
    }
}

#endif
