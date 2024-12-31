using ExcelRegistry.Shared.Helpers.Exceptions;

namespace ExcelRegistry.Domain.Regents.ValueObjects;

public class RegentSolutionBeingPrepared : ValueObject
{
    private RegentSolutionBeingPrepared(string solutionBeingPrepared)
    {
        Value = solutionBeingPrepared;
    }

    public string Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static RegentSolutionBeingPrepared Create(string solutionBeingPrepared)
    {
        Validate(solutionBeingPrepared);
        return new RegentSolutionBeingPrepared(solutionBeingPrepared);
    }

    private static void Validate(string solutionBeingPrepared)
    {
        if (string.IsNullOrEmpty(solutionBeingPrepared))
            throw new DomainException($"Ruošiamas tirpalas privalo būti nurodtas.");
    }
}