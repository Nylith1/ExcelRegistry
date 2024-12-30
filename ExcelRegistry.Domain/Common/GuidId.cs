using ExcelRegistry.Shared.Helpers.Exceptions;

namespace ExcelRegistry.Domain.Common;

public class GuidId : ValueObject
{
    private GuidId(string id)
    {
        Value = id;
    }

    public string Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static GuidId Create(string id)
    {
        Validate(id);
        return new GuidId(id);
    }

    private static void Validate(string id)
    {
        if (!Guid.TryParse(id, out Guid _))
        {
            throw new DomainException($"Id must be in guid format.");
        }
    }
}