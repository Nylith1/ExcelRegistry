using ExcelRegistry.Shared.Dtos.CosmosDb;
using MongoDB.Bson.Serialization.Attributes;

namespace ExcelRegistry.Access.CosmosDb.Models.DataModels;

public record UserData
{
    [BsonId]
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Picture { get; init; }
    public required IEnumerable<RoleData> Roles { get; init; }

    public UserDto MapToDto() => new() 
    { 
        Id = Id, 
        Name = Name, 
        Email = Email, 
        Picture = Picture, 
        Roles = Roles.Select(x => x.MapToDto()) 
    };
}
