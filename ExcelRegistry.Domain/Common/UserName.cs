using ExcelRegistry.Shared.Helpers.Exceptions;

namespace ExcelRegistry.Domain.Common;

public class UserName : ValueObject
{
    private const int MinUserNameSymbolCount = 3;
    private const int MaxUserNameSymbolCount = 250;

    private UserName(string name)
    {
        Value = name;
    }

    public string Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static UserName Create(string name)
    {
        ValidateName(name);
        return new UserName(name);
    }

    private static void ValidateName(string name)
    {
        if (name.Length < MinUserNameSymbolCount)
            throw new DomainException($"Vartotojo vardas turi sudaryti bent {MinUserNameSymbolCount} simboliai.");

        if (name.Length > MaxUserNameSymbolCount)
            throw new DomainException($"Vartotojo vardas turi sudaryti ne daugiau nei {MaxUserNameSymbolCount} simboliai.");
    }
}