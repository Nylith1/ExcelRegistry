using ExcelRegistry.Domain.Common;
using ExcelRegistry.Shared.Dtos.CosmosDb;
using ExcelRegistry.Shared.Helpers.Exceptions;

namespace ExcelRegistry.Domain.Users;

public class User
{
    private User(UserDomainDto domainDto)
    {
        Id = domainDto.Id;
        Name = domainDto.Name;
        Email = domainDto.Email;
        Picture = domainDto.Picture;
        Roles = domainDto.Roles;
    }

    public GuidId Id { get; private set; }
    public UserName Name { get; private set; }
    public UserEmail Email { get; private set; }
    public string Picture { get; private set; }
    public IReadOnlyCollection<Role> Roles { get; private set; }

    public static User Create(UserDto dto)
    {
        var domainDto = new UserDomainDto()
        {
            Id = GuidId.Create(dto.Id),
            Email = UserEmail.Create(dto.Email),
            Name = UserName.Create(dto.Name),
            Picture = dto.Picture,
            Roles = dto.Roles.Select(Role.Create).ToList()
        };

        ValidateRoles(domainDto.Roles);
        return new User(domainDto);
    }

    public void AddRole(RoleDto role)
    {
        if (Roles.Any(existingRole => existingRole.Id.Value == role.Id))
            throw new DomainException($"Rolė su ID {role.Id} jau egzistuoja.");

        var updatedRoles = Roles.ToList();
        updatedRoles.Add(Role.Create(role));

        Roles = updatedRoles.AsReadOnly();
    }

    public void RemoveRole(GuidId roleId)
    {
        if (string.IsNullOrWhiteSpace(roleId.Value))
            throw new DomainException("Rolės Id privalo būti nurodyta.");

        var updatedRoles = Roles.Where(role => role.Id.Value != roleId.Value).ToList();

        if (updatedRoles.Count == 0)
            throw new DomainException("Vartotojas privalo turėti bent vieną rolę");

        Roles = updatedRoles.AsReadOnly();
    }

    private static void ValidateRoles(IReadOnlyCollection<Role> roles)
    {
        if (roles.Count == 0)
            throw new DomainException($"Vartotojas neturi nei vienos rolės.");

        var duplicateRoles = roles
        .GroupBy(role => role.Id)
        .Where(group => group.Count() > 1)
        .Select(group => group.Key)
        .ToList();

        if (duplicateRoles.Count != 0)
            throw new DomainException($"Rolės negali dublikuotis: {string.Join(", ", duplicateRoles)}");

    }

    public UserDto MapToDto() => new()
    {
        Id = Id.Value,
        Name = Name.Value,
        Email = Email.Value,
        Picture = Picture,
        Roles = Roles.Select(x => new RoleDto { Id = x.Id.Value, Name = x.Name.Value })
    };
}

public record UserDomainDto
{
    public required GuidId Id { get; init; }
    public required UserName Name { get; init; }
    public required UserEmail Email { get; init; }
    public required string Picture { get; init; }
    public required IReadOnlyCollection<Role> Roles { get; init; }
}