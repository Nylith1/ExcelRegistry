using ExcelRegistry.Shared.Helpers.Exceptions;

namespace ExcelRegistry.Domain.Users.ValueObjects;

public class RoleName : ValueObject
{
    private const int MinRoleNameSymbolCount = 3;
    private const int MaxRoleNameSymbolCount = 250;

    private RoleName(string name)
    {
        Value = name;
    }

    public string Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static RoleName Create(string name)
    {
        Validate(name);
        return new RoleName(name);
    }

    private static void Validate(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new DomainException("Rolės pavadinimas negali būti tuščias.");

        if (name.Length < MinRoleNameSymbolCount)
            throw new DomainException($"Rolės pavadinimą turi sudaryti bent {MinRoleNameSymbolCount} simboliai.");

        if (name.Length > MaxRoleNameSymbolCount)
            throw new DomainException($"Rolės pavadinimą turi sudaryti ne daugiau nei {MaxRoleNameSymbolCount} simboliai.");
    }
}