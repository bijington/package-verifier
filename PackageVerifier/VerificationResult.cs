namespace PackageVerifier;

public class VerificationResult
{
    public IReadOnlyList<string> MissingMonikers { get; }
    public IReadOnlyList<string> ExtraMonikers { get; }
    public IReadOnlyList<string> ExpectedMonikers { get; }

    public VerificationResult(
        IReadOnlyList<string> missingMonikers,
        IReadOnlyList<string> extraMonikers,
        IReadOnlyList<string> expectedMonikers)
    {
        MissingMonikers = missingMonikers;
        ExtraMonikers = extraMonikers;
        ExpectedMonikers = expectedMonikers;
    }
}