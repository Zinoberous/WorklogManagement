using Microsoft.JSInterop;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;

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

        // Das entdprechende Paket für BrowserConsole funktioniert nur bei WASM und als Sink für Serilog hat es auch nicht funktioniert
        LogToBrowserConsole(logLevel, state, exception, formatter);
    }

    // TODO: verschachtelte Elemente (z.B.: Ticket { TicketAttachments: [] }) werden nicht korrekt aufgelöst
    private void LogToBrowserConsole<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        StringWriter writer = new();

        var logEvent = CreateLogEvent(logLevel, state, exception, formatter);

        var consoleMethod = SelectConsoleMethod(logEvent.Level);

        new MessageTemplateTextFormatter("[{Level:u3}] {Message:lj}{NewLine}{Exception}").Format(logEvent, writer);

        var message = writer.ToString().Trim();

        _ = Task.Run(async () => await _jsRuntime.InvokeAsync<string>(consoleMethod, message));
    }

    private LogEvent CreateLogEvent<TState>(
        LogLevel logLevel,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        var level = MapLogLevel(logLevel);

        return new(
            timestamp: _timeProvider.GetLocalNow(),
            level: level,
            exception: exception,
            messageTemplate: new MessageTemplateParser().Parse(message),
            properties: []);
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
