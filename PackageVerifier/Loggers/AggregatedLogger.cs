using Microsoft.Extensions.Logging;

namespace PackageVerifier.Loggers;

public class AggregatedLogger : ILogger
{
    public AggregatedLogger(Options options)
    {
        _missingMonikerLevel = GetLogLevel(options.TreatMissingTargetsAs);
        _extraMonikerLevel = GetLogLevel(options.TreatExtraTargetsAs);
        _loggers = options.Loggers.Select(logger => GetLogger(logger, _missingMonikerLevel, _extraMonikerLevel)).ToList();
    }
    
    private readonly List<IResultLogger> _loggers;
    private readonly LogLevel _missingMonikerLevel;
    private readonly LogLevel _extraMonikerLevel;
    
    private static IResultLogger GetLogger(Logger logger, LogLevel missingMonikerLevel, LogLevel extraMonikerLevel) =>
        logger switch
        {
            // Logger.Console => new ConsoleLogger(),
            Logger.AzureDevOps => new AzureDevOpsLogger(missingMonikerLevel, extraMonikerLevel),
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
        _loggers.ForEach(logger => logger.Log(verificationResult));
    }
}