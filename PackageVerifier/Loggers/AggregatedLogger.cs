using Microsoft.Extensions.Logging;

namespace PackageVerifier.Loggers;

public class AggregatedLogger : ILogger
{
    public AggregatedLogger(Options options)
    {
        _loggers = options.Loggers.Select(GetLogger).ToList();
        _missingMonikerLevel = GetLogLevel(options.TreatMissingTargetsAs);
        _extraMonikerLevel = GetLogLevel(options.TreatExtraTargetsAs);
    }
    
    private readonly List<ILogger> _loggers;
    private readonly LogLevel _missingMonikerLevel;
    private readonly LogLevel _extraMonikerLevel;
    
    private static ILogger GetLogger(Logger logger) =>
        logger switch
        {
            // Logger.Console => new ConsoleLogger(),
            Logger.AzureDevOps => new AzureDevOpsLogger(),
            //Logger.GitHubActions => new GitHubActionsLogger(),
            _ => throw new ArgumentOutOfRangeException(nameof(logger), logger, null)
        };

    private static LogLevel GetLogLevel(TargetFrameworkMonikerTreatmentRule rule) =>
        rule switch
        {
            TargetFrameworkMonikerTreatmentRule.Ignore => LogLevel.Information,
            TargetFrameworkMonikerTreatmentRule.Warning => LogLevel.Warning,
            TargetFrameworkMonikerTreatmentRule.Error => LogLevel.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
        };

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _loggers.ForEach(logger => logger.Log(logLevel, eventId, state, exception, formatter));
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
            _loggers.ForEach(logger => logger.Log(_extraMonikerLevel, "TFM '{extraDirectory}' was not expected", extraDirectory)); 
        }

        foreach (var missingDirectory in verificationResult.MissingMonikers)
        {
            _loggers.ForEach(logger => logger.Log(_missingMonikerLevel, "TFM '{missingDirectory}' is missing", missingDirectory)); 
        }

        foreach (var expectedDirectory in verificationResult.ExpectedMonikers)
        {
            _loggers.ForEach(logger => logger.LogInformation("Expected TFM '{expectedDirectory}' exists", expectedDirectory));
        }
    }
}