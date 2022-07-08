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
using System.Runtime.CompilerServices;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

// ReSharper disable MemberCanBePrivate.Global

namespace Serilog.Extensions.Hosting
{
    /// <summary>
    /// A Serilog <see cref="ILogger"/> that can be reconfigured without invalidating existing <see cref="ILogger"/>
    /// instances derived from it.
    /// </summary>
    public sealed class ReloadableLogger : ILogger, IReloadableLogger, IDisposable
    {
        readonly object _sync = new object();
        Logger _logger;
        
        // One-way; if the value is `true` it can never again be made `false`, allowing "double-checked" reads. If
        // `true`, `_logger` is final and a memory barrier ensures the final value is seen by all threads.
        bool _frozen;

        // Unsure whether this should be exposed; currently going for minimal API surface.
        internal ReloadableLogger(Logger initial)
        {
            _logger = initial ?? throw new ArgumentNullException(nameof(initial));
        }
        
        ILogger IReloadableLogger.ReloadLogger()
        {
            return _logger;
        }

        /// <summary>
        /// Reload the logger using the supplied configuration delegate.
        /// </summary>
        /// <param name="configure">A callback in which the logger is reconfigured.</param>
        /// <exception cref="ArgumentNullException"><paramref name="configure"/> is null.</exception>
        public void Reload(Func<LoggerConfiguration, LoggerConfiguration> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            
            lock (_sync)
            {
                _logger.Dispose();
                _logger = configure(new LoggerConfiguration()).CreateLogger();
            }
        }

        /// <summary>
        /// Freeze the logger, so that no further reconfiguration is possible. Once the logger is frozen, logging through
        /// new contextual loggers will have no additional cost, and logging directly through this logger will not require
        /// any synchronization.
        /// </summary>
        /// <returns>The <see cref="Logger"/> configured with the final settings.</returns>
        /// <exception cref="InvalidOperationException">The logger is already frozen.</exception>
        public Logger Freeze()
        {
            lock (_sync)
            {
                if (_frozen)
                    throw new InvalidOperationException("The logger is already frozen.");

                _frozen = true;

                // https://github.com/dotnet/runtime/issues/20500#issuecomment-284774431
                // Publish `_logger` and `_frozen`. This is useful here because it means that once the logger is frozen - which
                // we always expect - reads don't require any synchronization/interlocked instructions.
                Interlocked.MemoryBarrierProcessWide();
                
                return _logger;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            lock (_sync)
                _logger.Dispose();
        }

        /// <inheritdoc />
        public ILogger ForContext(ILogEventEnricher enricher)
        {
            if (enricher == null) return this;
            
            if (_frozen)
                return _logger.ForContext(enricher);

            lock (_sync)
                return new CachingReloadableLogger(this, _logger, this, p => p.ForContext(enricher));
        }

        /// <inheritdoc />
        public ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
        {
            if (enrichers == null) return this;
            
            if (_frozen)
                return _logger.ForContext(enrichers);

            lock (_sync)
                return new CachingReloadableLogger(this, _logger, this, p => p.ForContext(enrichers));
        }

        /// <inheritdoc />
        public ILogger ForContext(string propertyName, object value, bool destructureObjects = false)
        {
            if (propertyName == null) return this;
            
            if (_frozen)
                return _logger.ForContext(propertyName, value, destructureObjects);

            lock (_sync)
                return new CachingReloadableLogger(this, _logger, this, p => p.ForContext(propertyName, value, destructureObjects));
        }

        /// <inheritdoc />
        public ILogger ForContext<TSource>()
        {
            if (_frozen)
                return _logger.ForContext<TSource>();

            lock (_sync)
                return new CachingReloadableLogger(this, _logger, this, p => p.ForContext<TSource>());
        }

        /// <inheritdoc />
        public ILogger ForContext(Type source)
        {
            if (source == null) return this;
            
            if (_frozen)
                return _logger.ForContext(source);

            lock (_sync)
                return new CachingReloadableLogger(this, _logger, this, p => p.ForContext(source));
        }

        /// <inheritdoc />
        public void Write(LogEvent logEvent)
        {
            if (_frozen)
            {
                _logger.Write(logEvent);
                return;
            }

            lock (_sync)
            {
                _logger.Write(logEvent);
            }
        }

        /// <inheritdoc />
        public void Write(LogEventLevel level, string messageTemplate)
        {
            if (_frozen)
            {
                _logger.Write(level, messageTemplate);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, messageTemplate);
            }
        }

        /// <inheritdoc />
        public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue)
        {
            if (_frozen)
            {
                _logger.Write(level, messageTemplate, propertyValue);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, messageTemplate, propertyValue);
            }
        }

        /// <inheritdoc />
        public void Write<T0, T1>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            if (_frozen)
            {
                _logger.Write(level, messageTemplate, propertyValue0, propertyValue1);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, messageTemplate, propertyValue0, propertyValue1);
            }
        }

        /// <inheritdoc />
        public void Write<T0, T1, T2>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            if (_frozen)
            {
                _logger.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
            }
        }

        /// <inheritdoc />
        public void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues)
        {
            if (_frozen)
            {
                _logger.Write(level, messageTemplate, propertyValues);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, messageTemplate, propertyValues);
            }
        }

        /// <inheritdoc />
        public void Write(LogEventLevel level, Exception exception, string messageTemplate)
        {
            if (_frozen)
            {
                _logger.Write(level, exception, messageTemplate);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, exception, messageTemplate);
            }
        }

        /// <inheritdoc />
        public void Write<T>(LogEventLevel level, Exception exception, string messageTemplate, T propertyValue)
        {
            if (_frozen)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue);
            }
        }

        /// <inheritdoc />
        public void Write<T0, T1>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            if (_frozen)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
            }
        }

        /// <inheritdoc />
        public void Write<T0, T1, T2>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            if (_frozen)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
            }
        }

        /// <inheritdoc />
        public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (_frozen)
            {
                _logger.Write(level, exception, messageTemplate, propertyValues);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, exception, messageTemplate, propertyValues);
            }
        }

        /// <inheritdoc />
        public bool IsEnabled(LogEventLevel level)
        {
            if (_frozen)
            {
                return _logger.IsEnabled(level);
            }

            lock (_sync)
            {
                return _logger.IsEnabled(level);
            }
        }
        
        /// <inheritdoc />
        public bool BindMessageTemplate(string messageTemplate, object[] propertyValues, out MessageTemplate parsedTemplate,
            out IEnumerable<LogEventProperty> boundProperties)
        {
            if (_frozen)
            {
                return _logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
            }

            lock (_sync)
            {
                return _logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
            }
        }

        /// <inheritdoc />
        public bool BindProperty(string propertyName, object value, bool destructureObjects, out LogEventProperty property)
        {
            if (_frozen)
            {
                return _logger.BindProperty(propertyName, value, destructureObjects, out property);
            }

            lock (_sync)
            {
                return _logger.BindProperty(propertyName, value, destructureObjects, out property);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        (ILogger, bool) UpdateForCaller(ILogger root, ILogger cached, IReloadableLogger caller, out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            // Synchronization on `_sync` is not required in this method; it will be called without a lock
            // if `_frozen` and under a lock if not.
            
            if (_frozen)
            {
                // If we're frozen, then the caller hasn't observed this yet and should update. We could optimize a little here
                // and only signal an update if the cached logger is stale (as per the next condition below).
                newRoot = _logger;
                newCached = caller.ReloadLogger();
                frozen = true;
                return (newCached, true);
            }

            if (cached != null && root == _logger)
            {
                newRoot = default;
                newCached = default;
                frozen = false;
                return (cached, false);
            }
        
            newRoot = _logger;
            newCached = caller.ReloadLogger();
            frozen = false;
            return (newCached, true);
        }
        
        internal bool InvokeIsEnabled(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, out bool isEnabled, out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                isEnabled = logger.IsEnabled(level);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                isEnabled = logger.IsEnabled(level);
                return update;
            }
        }
        
        internal bool InvokeBindMessageTemplate(ILogger root, ILogger cached, IReloadableLogger caller, string messageTemplate, 
            object[] propertyValues, out MessageTemplate parsedTemplate, out IEnumerable<LogEventProperty> boundProperties,
            out bool canBind, out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                canBind = logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                canBind = logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
                return update;
            }
        }
        
        internal bool InvokeBindProperty(ILogger root, ILogger cached, IReloadableLogger caller, string propertyName, 
            object propertyValue, bool destructureObjects, out LogEventProperty property,
            out bool canBind, out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                canBind = logger.BindProperty(propertyName, propertyValue, destructureObjects, out property);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                canBind = logger.BindProperty(propertyName, propertyValue, destructureObjects, out property);
                return update;
            }
        }

        internal bool InvokeWrite(ILogger root, ILogger cached, IReloadableLogger caller, LogEvent logEvent, out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(logEvent);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(logEvent);
                return update;
            }
        }

        internal bool InvokeWrite(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, string messageTemplate,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate);
                return update;
            }
        }

        internal bool InvokeWrite<T>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, string messageTemplate,
            T propertyValue,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue);
                return update;
            }
        }
        
        internal bool InvokeWrite<T0, T1>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, string messageTemplate,
            T0 propertyValue0, T1 propertyValue1,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue0, propertyValue1);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue0, propertyValue1);
                return update;
            }
        }
        
        internal bool InvokeWrite<T0, T1, T2>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, string messageTemplate,
            T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return update;
            }
        }

        internal bool InvokeWrite(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, string messageTemplate,
            object[] propertyValues,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValues);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValues);
                return update;
            }
        }
        
        internal bool InvokeWrite(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, Exception exception, string messageTemplate,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate);
                return update;
            }
        }

        internal bool InvokeWrite<T>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, Exception exception, string messageTemplate,
            T propertyValue,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue);
                return update;
            }
        }
        
        internal bool InvokeWrite<T0, T1>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, Exception exception, string messageTemplate,
            T0 propertyValue0, T1 propertyValue1,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
                return update;
            }
        }
        
        internal bool InvokeWrite<T0, T1, T2>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, Exception exception, string messageTemplate,
            T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return update;
            }
        }

        internal bool InvokeWrite(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, Exception exception, string messageTemplate,
            object[] propertyValues,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValues);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValues);
                return update;
            }
        }

        internal bool CreateChild(
            ILogger root, 
            IReloadableLogger parent, 
            ILogger cachedParent,
            Func<ILogger, ILogger> configureChild,
            out ILogger child,
            out ILogger newRoot,
            out ILogger newCached,
            out bool frozen)
        {
            if (_frozen)
            {
                var (logger, _) = UpdateForCaller(root, cachedParent, parent, out newRoot, out newCached, out frozen);
                child = configureChild(logger);
                return true; // Always an update, since the caller has not observed that the reloadable logger is frozen.
            }

            // No synchronization, here - a lot of loggers are created and thrown away again without ever being used,
            // so we just return a lazy wrapper.
            child = new CachingReloadableLogger(this, root, parent, configureChild);
            newRoot = default;
            newCached = default;
            frozen = default;
            return false;
        }
    }
}

#endif
