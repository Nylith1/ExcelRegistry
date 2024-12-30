using ExcelRegistry.Shared.Dtos.CosmosDb;
using MongoDB.Bson.Serialization.Attributes;

namespace ExcelRegistry.Access.CosmosDb.Models.DataModels;

public record RoleData
{
    [BsonId]
    public required string Id { get; init; }
    public required string Name { get; init; }

    public RoleDto MapToDto() => new() { Id = Id, Name = Name };
}
