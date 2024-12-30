namespace ExcelRegistry.Shared.Dtos.CosmosDb;

public record UserDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string Picture { get; init; } = string.Empty;
    public required IEnumerable<RoleDto> Roles { get; init; }
}
