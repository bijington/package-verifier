using Microsoft.Extensions.Logging;

namespace PackageVerifier.Loggers;

public interface IResultLogger : ILogger
{
    void Log(VerificationResult verificationResult);
}