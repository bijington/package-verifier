namespace PackageVerifier;

public class VerificationResult
{
    public IReadOnlyList<string> MissingMonikers { get; }
    public IReadOnlyList<string> ExtraMonikers { get; }
    public IReadOnlyList<string> DetectedMonikers { get; }

    public VerificationResult(
        IReadOnlyList<string> missingMonikers,
        IReadOnlyList<string> extraMonikers,
        IReadOnlyList<string> detectedMonikers)
    {
        MissingMonikers = missingMonikers;
        ExtraMonikers = extraMonikers;
        DetectedMonikers = detectedMonikers;
    }
}