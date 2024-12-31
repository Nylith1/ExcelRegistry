namespace ExcelRegistry.Domain.Regents.ValueObjects;

public class RegentDate : ValueObject
{
    private RegentDate(DateTime date)
    {
        Value = date;
    }

    public DateTime Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static RegentDate Create(DateTime date, int timeZoneOffsetInHours)
    {
        Validate(date);
        return new RegentDate(date.AddHours(timeZoneOffsetInHours));
    }

    private static void Validate(DateTime date)
    {
    }
}