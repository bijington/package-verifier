using Microsoft.Extensions.Logging;

namespace PackageVerifier.Loggers;

public class AzureDevOpsLogger : ILogger
{
    private static string GetPrefix(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => string.Empty,
            LogLevel.Debug => "##[debug]",
            LogLevel.Information => string.Empty,
            LogLevel.Warning => "##[warning]",
            LogLevel.Error => "##[error]",
            LogLevel.Critical => "##[error]",
            LogLevel.None => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Console.WriteLine($"{GetPrefix(logLevel)}{formatter(state, exception)}");
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public void Log(VerificationResult verificationResult)
    {
        
    }
}