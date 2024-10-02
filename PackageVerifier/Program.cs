using System.IO.Compression;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace PackageVerifier;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                var extractDirectory = Path.Join(Environment.CurrentDirectory, "abc");
                
                var loggers = new List<ILogger>
                {
                    new AzureDevOpsLogger()
                };
                
                try
                {
                    loggers.ForEach(logger => logger.LogInformation("Verifying package '{PackagePath}'", o.PackagePath));
                    loggers.ForEach(logger => logger.LogInformation("Extracting package to directory '{extractDirectory}'", extractDirectory));

                    ZipFile.ExtractToDirectory(o.PackagePath, extractDirectory);

                    loggers.ForEach(logger => logger.LogInformation("Checking for TFMs: '{TargetFrameworkMonikers}'", string.Join(';', o.TargetFrameworkMonikers))); ;

                    var libDirectory = Path.Join(extractDirectory, "lib");
                    var directories = new DirectoryInfo(libDirectory).GetDirectories().Select(x => x.Name).ToList();

                    foreach (var extraDirectory in directories.Except(o.TargetFrameworkMonikers))
                    {
                        loggers.ForEach(logger => logger.Log(GetLogLevel(o.TreatExtraTargetsAs), "TFM '{extraDirectory}' was not expected", extraDirectory)); 
                    }

                    foreach (var missingDirectory in o.TargetFrameworkMonikers.Except(directories))
                    {
                        loggers.ForEach(logger => logger.Log(GetLogLevel(o.TreatMissingTargetsAs), "TFM '{missingDirectory}' is missing", missingDirectory)); 
                    }

                    foreach (var expectedDirectory in o.TargetFrameworkMonikers.Intersect(directories))
                    {
                        loggers.ForEach(logger => logger.LogInformation("Expected TFM '{expectedDirectory}' exists", expectedDirectory));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    try
                    {
                        Console.WriteLine($@"Cleaning up '{extractDirectory}'");
                        
                        Directory.Delete(extractDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });
    }

    private static LogLevel GetLogLevel(TargetFrameworkMonikerTreatmentRule rule) =>
        rule switch
        {
            TargetFrameworkMonikerTreatmentRule.Ignore => LogLevel.Information,
            TargetFrameworkMonikerTreatmentRule.Warning => LogLevel.Warning,
            TargetFrameworkMonikerTreatmentRule.Error => LogLevel.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
        };
}