using Microsoft.Extensions.Logging;

namespace PackageVerifier.Loggers;

public class AzureDevOpsLogger : IResultLogger
{
    public AzureDevOpsLogger(LogLevel missingMonikerLevel, LogLevel extraMonikerLevel)
    {
        _missingMonikerLevel = missingMonikerLevel;
        _extraMonikerLevel = extraMonikerLevel;
    }
    
    private readonly LogLevel _missingMonikerLevel;
    private readonly LogLevel _extraMonikerLevel;
    
    private static string GetPrefix(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => string.Empty,
            LogLevel.Debug => "##[debug]",
            LogLevel.Information => string.Empty,
            LogLevel.Warning => "##[warning]", //##vso[task.logissue type=warning;]
            LogLevel.Error => "##[error]", //##vso[task.logissue type=error;]
            LogLevel.Critical => "##[error]", //##vso[task.logissue type=error;]
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
        foreach (var extraDirectory in verificationResult.ExtraMonikers)
        {
            this.Log(_extraMonikerLevel, "TFM '{extraDirectory}' was not expected", extraDirectory); 
        }

        foreach (var missingDirectory in verificationResult.MissingMonikers)
        {
            this.Log(_missingMonikerLevel, "TFM '{missingDirectory}' is missing", missingDirectory); 
        }

        foreach (var expectedDirectory in verificationResult.DetectedMonikers)
        {
            this.LogInformation("Expected TFM '{expectedDirectory}' exists", expectedDirectory);
        }
        
        if (verificationResult.MissingMonikers.Any() &&
            _missingMonikerLevel >= LogLevel.Error)
        {
            this.LogInformation("##vso[task.complete result=Failed;]"); 
        }
        else if (verificationResult.ExtraMonikers.Any() &&
                 _extraMonikerLevel >= LogLevel.Error)
        {
            this.LogInformation("##vso[task.complete result=Failed;]");
        }
        else
        {
            this.LogInformation("##vso[task.complete result=Succeeded;]DONE");
        }
    }
}