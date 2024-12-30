using ExcelRegistry.Domain.Common;
using ExcelRegistry.Shared.Dtos.CosmosDb;

namespace ExcelRegistry.Domain.Users;

public class Role
{
    private Role(RoleDomainDto domainDto)
    {
        Id = domainDto.Id;
        Name = domainDto.Name;
    }

    public GuidId Id { get; private set; }
    public RoleName Name { get; private set; }

    public static Role Create(RoleDto dto)
    {
        var domainDto = new RoleDomainDto()
        {
            Id = GuidId.Create(dto.Id),
            Name = RoleName.Create(dto.Name),
        };

        return new Role(domainDto);
    }
}

public record RoleDomainDto
{
    public required GuidId Id { get; init; }
    public required RoleName Name { get; init; }
}
