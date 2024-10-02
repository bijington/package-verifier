using CommandLine;
using Microsoft.Extensions.Logging;

namespace PackageVerifier;

[Verb("verify")]
public class Options
{
    public Options(
        string packagePath,
        IEnumerable<string> targetFrameworkMonikers,
        TargetFrameworkMonikerTreatmentRule treatExtraTargetsAs,
        TargetFrameworkMonikerTreatmentRule treatMissingTargetsAs)
    {
        PackagePath = packagePath;
        TargetFrameworkMonikers = targetFrameworkMonikers;
        TreatExtraTargetsAs = treatExtraTargetsAs;
        TreatMissingTargetsAs = treatMissingTargetsAs;
    }
    
    [Option("package-path", Required = true, HelpText = "Path to the nuget package to inspect.")]
    public string PackagePath { get; }
    
    [Option("tfms", Separator = ';', Required = true, HelpText = "The TFMs expected to be included.")]
    public IEnumerable<string> TargetFrameworkMonikers { get; }
    
    [Option("treat-extra-tfms-as", Required = false, Default = TargetFrameworkMonikerTreatmentRule.Warning, HelpText = "Determines how to treat extra TFMs in the package.")]
    public TargetFrameworkMonikerTreatmentRule TreatExtraTargetsAs { get; }
    
    [Option("treat-missing-tfms-as", Required = false, Default = TargetFrameworkMonikerTreatmentRule.Error, HelpText = "Determines how to treat missing TFMs in the package.")]
    public TargetFrameworkMonikerTreatmentRule TreatMissingTargetsAs { get; }
}

public enum TargetFrameworkMonikerTreatmentRule
{
    Ignore,
    Warning,
    Error
}

public enum Logger
{
    Console,
    AzureDevOps,
    GitHubActions
}

public class AzureDevOpsLogger : ILogger
{
    private static string GetPrefix(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => string.Empty,
            LogLevel.Debug => "#[debug]",
            LogLevel.Information => string.Empty,
            LogLevel.Warning => "#[warning]",
            LogLevel.Error => "#[error]",
            LogLevel.Critical => "#[error]",
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
}