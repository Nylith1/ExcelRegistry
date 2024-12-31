using ExcelRegistry.Shared.Helpers.Exceptions;

namespace ExcelRegistry.Domain.Regents.ValueObjects;

public class RegentRowNumber : ValueObject
{
    private RegentRowNumber(string rowNumber)
    {
        Value = rowNumber;
    }

    public string Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static RegentRowNumber Create(string rowNumber)
    {
        Validate(rowNumber);
        return new RegentRowNumber(rowNumber);
    }

    private static void Validate(string rowNumber)
    {
        if (string.IsNullOrEmpty(rowNumber))
            throw new DomainException($"Eilės numeris privalo būti nurodytas.");
    }
}