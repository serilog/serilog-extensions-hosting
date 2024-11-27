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

using Serilog.Core;
using Serilog.Events;

namespace Serilog.Extensions.Hosting;

/// <summary>
/// Implements default <see cref="ILogger"/> methods for caching/reloadable loggers.
/// </summary>
public abstract class LoggerBase
{
    static readonly object[] NoPropertyValues = [];
    
    internal LoggerBase()
    {
    }
    
    /// <summary>
    /// Write an event to the log.
    /// </summary>
    /// <param name="logEvent">The event to write.</param>
    public abstract void Write(LogEvent logEvent);

    /// <summary>
    /// Write a log event with the specified level.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    [MessageTemplateFormatMethod("messageTemplate")]
    public abstract void Write(LogEventLevel level, string messageTemplate);

    /// <summary>
    /// Write a log event with the specified level.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    [MessageTemplateFormatMethod("messageTemplate")]
    public abstract void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue);
    
    /// <summary>
    /// Write a log event with the specified level.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    [MessageTemplateFormatMethod("messageTemplate")]
    public abstract void Write<T0, T1>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1);

    /// <summary>
    /// Write a log event with the specified level.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    [MessageTemplateFormatMethod("messageTemplate")]
    public abstract void Write<T0, T1, T2>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2);

    /// <summary>
    /// Write a log event with the specified level.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="messageTemplate"></param>
    /// <param name="propertyValues"></param>
    [MessageTemplateFormatMethod("messageTemplate")]
    public abstract void Write(LogEventLevel level, string messageTemplate, params object?[]? propertyValues);

    /// <summary>
    /// Write a log event with the specified level and associated exception.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    [MessageTemplateFormatMethod("messageTemplate")]
    public abstract void Write(LogEventLevel level, Exception? exception, string messageTemplate);

    /// <summary>
    /// Write a log event with the specified level and associated exception.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    [MessageTemplateFormatMethod("messageTemplate")]
    public abstract void Write<T>(LogEventLevel level, Exception? exception, string messageTemplate, T propertyValue);

    /// <summary>
    /// Write a log event with the specified level and associated exception.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    [MessageTemplateFormatMethod("messageTemplate")]
    public abstract void Write<T0, T1>(LogEventLevel level, Exception? exception, string messageTemplate, T0 propertyValue0,
        T1 propertyValue1);

    /// <summary>
    /// Write a log event with the specified level and associated exception.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    [MessageTemplateFormatMethod("messageTemplate")]
    public abstract void Write<T0, T1, T2>(LogEventLevel level, Exception? exception, string messageTemplate,
        T0 propertyValue0, T1 propertyValue1, T2 propertyValue2);

    /// <summary>
    /// Write a log event with the specified level and associated exception.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    [MessageTemplateFormatMethod("messageTemplate")]
    public abstract void Write(LogEventLevel level, Exception? exception, string messageTemplate, params object?[]? propertyValues);
    
    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Verbose(string messageTemplate)
        => Write(LogEventLevel.Verbose, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Verbose<T>(string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Verbose, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Verbose<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Write(LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Verbose(string messageTemplate, params object?[]? propertyValues)
        => Verbose((Exception?)null, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Verbose(Exception? exception, string messageTemplate)
        => Write(LogEventLevel.Verbose, exception, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Verbose<T>(Exception? exception, string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Verbose<T0, T1>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Verbose<T0, T1, T2>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
        => Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Verbose(Exception? exception, string messageTemplate, params object?[]? propertyValues)
        => Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Debug(string messageTemplate)
        => Write(LogEventLevel.Debug, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Debug<T>(string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Debug, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Write(LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Debug(string messageTemplate, params object?[]? propertyValues)
        => Debug((Exception?)null, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Debug(Exception? exception, string messageTemplate)
        => Write(LogEventLevel.Debug, exception, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Debug<T>(Exception? exception, string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Debug, exception, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Debug<T0, T1>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Debug<T0, T1, T2>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
        => Write(LogEventLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Debug(Exception? exception, string messageTemplate, params object?[]? propertyValues)
        => Write(LogEventLevel.Debug, exception, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Information(string messageTemplate)
        => Write(LogEventLevel.Information, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Information<T>(string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Information, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Information<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Information<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Write(LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Information(string messageTemplate, params object?[]? propertyValues)
        => Information((Exception?)null, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Information(Exception? exception, string messageTemplate)
        => Write(LogEventLevel.Information, exception, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Information<T>(Exception? exception, string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Information, exception, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Information<T0, T1>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Information, exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Information<T0, T1, T2>(Exception? exception, string messageTemplate, T0 propertyValue0,
        T1 propertyValue1, T2 propertyValue2)
        => Write(LogEventLevel.Information, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Information(Exception? exception, string messageTemplate, params object?[]? propertyValues)
        => Write(LogEventLevel.Information, exception, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Warning(string messageTemplate)
        => Write(LogEventLevel.Warning, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Warning<T>(string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Warning, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Warning<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Warning<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Write(LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Warning(string messageTemplate, params object?[]? propertyValues)
        => Warning((Exception?)null, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Warning(Exception? exception, string messageTemplate)
        => Write(LogEventLevel.Warning, exception, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Warning<T>(Exception? exception, string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Warning, exception, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Warning<T0, T1>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Warning, exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Warning<T0, T1, T2>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
        => Write(LogEventLevel.Warning, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Warning(Exception? exception, string messageTemplate, params object?[]? propertyValues)
        => Write(LogEventLevel.Warning, exception, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Error(string messageTemplate)
        => Write(LogEventLevel.Error, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Error<T>(string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Error, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Write(LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Error(string messageTemplate, params object?[]? propertyValues)
        => Error((Exception?)null, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Error(Exception? exception, string messageTemplate)
        => Write(LogEventLevel.Error, exception, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Error<T>(Exception? exception, string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Error, exception, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Error<T0, T1>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Error<T0, T1, T2>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
        => Write(LogEventLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Error(Exception? exception, string messageTemplate, params object?[]? propertyValues)
        => Write(LogEventLevel.Error, exception, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Fatal("Process terminating.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Fatal(string messageTemplate)
        => Write(LogEventLevel.Fatal, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Fatal("Process terminating.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Fatal<T>(string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Fatal, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Fatal("Process terminating.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Fatal("Process terminating.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Fatal<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        => Write(LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Fatal("Process terminating.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Fatal(string messageTemplate, params object?[]? propertyValues)
        => Fatal((Exception?)null, messageTemplate, propertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example><code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Fatal(Exception? exception, string messageTemplate)
        => Write(LogEventLevel.Fatal, exception, messageTemplate, NoPropertyValues);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Fatal<T>(Exception? exception, string messageTemplate, T propertyValue)
        => Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValue);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Fatal<T0, T1>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        => Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValue0">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue1">Object positionally formatted into the message template.</param>
    /// <param name="propertyValue2">Object positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Fatal<T0, T1, T2>(Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
        T2 propertyValue2)
        => Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
    /// <example><code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code></example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public void Fatal(Exception? exception, string messageTemplate, params object?[]? propertyValues)
        => Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValues);
}