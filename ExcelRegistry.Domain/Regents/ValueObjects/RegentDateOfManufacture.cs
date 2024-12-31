using ExcelRegistry.Shared.Helpers.Exceptions;

namespace ExcelRegistry.Domain.Regents.ValueObjects;

public class RegentDateOfManufacture : ValueObject
{
    private RegentDateOfManufacture(DateTime regentDateOfManufacture)
    {
        Value = regentDateOfManufacture;
    }

    public DateTime Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static RegentDateOfManufacture Create(DateTime regentDateOfManufacture, int timeZoneOffsetInHours)
    {
        Validate(regentDateOfManufacture);
        return new RegentDateOfManufacture(regentDateOfManufacture.AddHours(timeZoneOffsetInHours));
    }

    private static void Validate(DateTime regentDateOfManufacture)
    {
        if (regentDateOfManufacture.Date > DateTime.UtcNow.Date)
            throw new DomainException("Pagaminimo data negali būti ateityje.");
    }
}