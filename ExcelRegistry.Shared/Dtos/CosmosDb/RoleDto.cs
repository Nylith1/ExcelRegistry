namespace ExcelRegistry.Shared.Dtos.CosmosDb;

public record RoleDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}
