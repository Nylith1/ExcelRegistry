namespace ExcelRegistry.Domain.Regents.ValueObjects;

public class RegentExpireDate : ValueObject
{
    private RegentExpireDate(DateTime expireDate)
    {
        Value = expireDate;
    }

    public DateTime Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static RegentExpireDate Create(DateTime expireDate, int timeZoneOffsetInHours)
    {
        Validate(expireDate);
        return new RegentExpireDate(expireDate.AddHours(timeZoneOffsetInHours));
    }

    private static void Validate(DateTime expireDate)
    {
    }
}