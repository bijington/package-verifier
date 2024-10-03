using System.IO.Compression;
using CommandLine;
using Microsoft.Extensions.Logging;
using PackageVerifier.Loggers;

namespace PackageVerifier;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                var extractDirectory = Path.Join(Environment.CurrentDirectory, "abc");

                var logger = new AggregatedLogger(o);
                
                try
                {
                    logger.LogInformation("Verifying package '{PackagePath}'", o.PackagePath);
                    logger.LogInformation("Extracting package to directory '{extractDirectory}'", extractDirectory);

                    ZipFile.ExtractToDirectory(o.PackagePath, extractDirectory);

                    logger.LogInformation("Checking for TFMs: '{TargetFrameworkMonikers}'", string.Join(';', o.TargetFrameworkMonikers));

                    var libDirectory = Path.Join(extractDirectory, "lib");
                    var directories = new DirectoryInfo(libDirectory).GetDirectories().Select(x => x.Name).ToList();

                    var result = new VerificationResult(
                        o.TargetFrameworkMonikers.Except(directories).ToList(),
                        directories.Except(o.TargetFrameworkMonikers).ToList(),
                        o.TargetFrameworkMonikers.Intersect(directories).ToList());
                    
                    logger.Log(result);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to verify package.");
                    throw;
                }
                finally
                {
                    try
                    {
                        logger.LogInformation("Cleaning up '{extractDirectory}'", extractDirectory);
                        
                        Directory.Delete(extractDirectory, true);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to clean up '{extractDirectory}'", extractDirectory);
                    }
                }
            });
    }
}