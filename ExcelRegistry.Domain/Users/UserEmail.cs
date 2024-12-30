using ExcelRegistry.Shared.Helpers.Exceptions;
using System.Text.RegularExpressions;

namespace ExcelRegistry.Domain.Users;

public class UserEmail : ValueObject
{
    private UserEmail(string email)
    {
        Value = email;
    }

    public string Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static UserEmail Create(string email)
    {
        Validate(email);
        return new UserEmail(email);
    }

    private static void Validate(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new DomainException($"Elektroninis paštas privalo būti nurodytas.");

        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailPattern))
            throw new DomainException($"Elektroninis paštas nurodytas neteisingai.");
    }
}