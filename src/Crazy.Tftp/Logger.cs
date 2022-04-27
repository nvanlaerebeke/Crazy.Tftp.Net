using Microsoft.Extensions.Logging;

namespace Crazy.Tftp;

internal static class Logger
{
    private static readonly ILoggerFactory _logFactory;

    static Logger()
    {
        _logFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                .AddConsole();
        });
    }

    public static ILogger<T> Get<T>()
    {
        return _logFactory.CreateLogger<T>();
    }
}