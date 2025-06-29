using Microsoft.JSInterop;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace WorklogManagement.UI.Services;

public interface ILoggerService<T> : ILogger<T>;

public class LoggerService<T>(ILoggerFactory loggerFactory, IJSRuntime jsRuntime, TimeProvider timeProvider) : ILoggerService<T>
{
    private readonly ILogger<T> _logger = loggerFactory.CreateLogger<T>();
    private readonly IJSRuntime _jsRuntime = jsRuntime;
    private readonly TimeProvider _timeProvider = timeProvider;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _logger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _logger.Log(logLevel, eventId, state, exception, formatter);

        // Das entsprechende Paket für BrowserConsole funktioniert nur bei WASM und Sinks für Serilog unterstützen kein Dependency Injection für IJSRuntime
        _ = LogToBrowserConsole(logLevel, state, exception);
    }

    private async Task LogToBrowserConsole<TState>(LogLevel logLevel, TState state, Exception? exception)
    {
        StringWriter writer = new();

        var logEvent = CreateLogEvent(logLevel, state, exception);

        var consoleMethod = SelectConsoleMethod(logEvent.Level);

        new MessageTemplateTextFormatter("[{Level:u3}] {Message:lj}{NewLine}{Exception}").Format(logEvent, writer);

        var message = writer.ToString().Trim();

        await _jsRuntime.InvokeAsync<string>(consoleMethod, message);
    }

    private LogEvent CreateLogEvent<TState>(LogLevel logLevel, TState state, Exception? exception)
    {
        var level = MapLogLevel(logLevel);

        var stateDict = (state as IEnumerable<KeyValuePair<string, object?>>)!
            .ToDictionary(
                x => x.Key.TrimStart('@', '$'),
                x => Regex.Unescape(JsonSerializer.Serialize(x.Value).Trim('"')));

        var messageTemplate = new MessageTemplateParser().Parse(stateDict["{OriginalFormat}"]);

        stateDict.Remove("{OriginalFormat}");

        var properties = stateDict
            .Select(x => new LogEventProperty(x.Key, new ScalarValue(x.Value)))
            .ToArray();

        return new(
            timestamp: _timeProvider.GetLocalNow(),
            level: level,
            exception: exception,
            messageTemplate: messageTemplate,
            properties: properties);
    }

    private static LogEventLevel MapLogLevel(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            _ => LogEventLevel.Information
        };

    static string SelectConsoleMethod(LogEventLevel logLevel) =>
       logLevel switch
       {
           >= LogEventLevel.Error => "console.error",
           LogEventLevel.Warning => "console.warn",
           LogEventLevel.Information => "console.info",
           LogEventLevel.Debug => "console.debug",
           _ => "console.log"
       };
}

public class BrowserConsoleSink(IJSRuntime jsRuntime) : ILogEventSink
{
    private readonly IJSRuntime _jsRuntime = jsRuntime;

    public void Emit(LogEvent logEvent)
    {
        StringWriter writer = new();

        var consoleMethod = SelectConsoleMethod(logEvent.Level);

        new MessageTemplateTextFormatter("[{Level:u3}] {Message:lj}{NewLine}{Exception}").Format(logEvent, writer);

        var message = writer.ToString().Trim();

        _ = Task.Run(() => _jsRuntime.InvokeAsync<string>(consoleMethod, message));
    }

    static string SelectConsoleMethod(LogEventLevel logLevel) =>
        logLevel switch
        {
            >= LogEventLevel.Error => "console.error",
            LogEventLevel.Warning => "console.warn",
            LogEventLevel.Information => "console.info",
            LogEventLevel.Debug => "console.debug",
            _ => "console.log"
        };
}
