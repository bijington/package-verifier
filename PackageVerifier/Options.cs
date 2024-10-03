using CommandLine;

namespace PackageVerifier;

public class Options
{
    public Options(
        IEnumerable<Logger> loggers,
        string packagePath,
        IEnumerable<string> targetFrameworkMonikers,
        TargetFrameworkMonikerTreatmentRule treatExtraTargetsAs,
        TargetFrameworkMonikerTreatmentRule treatMissingTargetsAs)
    {
        Loggers = loggers;
        PackagePath = packagePath;
        TargetFrameworkMonikers = targetFrameworkMonikers;
        TreatExtraTargetsAs = treatExtraTargetsAs;
        TreatMissingTargetsAs = treatMissingTargetsAs;
    }
    
    [Option("loggers", Separator = ';', Required = true, HelpText = "The loggers to use when reporting results.")]
    public IEnumerable<Logger> Loggers { get; }
    
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
    Console = 1,
    AzureDevOps = 2,
    GitHubActions = 4
}