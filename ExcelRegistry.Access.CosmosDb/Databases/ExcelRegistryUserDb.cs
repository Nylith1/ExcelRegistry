using ExcelRegistry.Access.CosmosDb.Databases.Interfaces;
using ExcelRegistry.Shared;
using ExcelRegistry.Shared.Options;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Authentication;

namespace ExcelRegistry.Access.CosmosDb.Databases;

public class ExcelRegistryUserDb : IExcelRegistryUserDb
{
    private readonly IMongoDatabase db;

    public ExcelRegistryUserDb(IOptions<ExcelRegistryUserDbOptions> excelRegistryUserDbOptions)
    {
        MongoClientSettings settings = MongoClientSettings.FromUrl(new(excelRegistryUserDbOptions.Value.ConnectionString));
        settings.SslSettings = new() { EnabledSslProtocols = SslProtocols.Tls12 };
        var client = new MongoClient(settings);

        db = client.GetDatabase(Constants.Databases.ExcelRegistryUsersDb);
    }

    public async Task InsertRecordsAsync<T>(string table, T record, CancellationToken cancellationToken)
    {
        var collection = db.GetCollection<T>(table);
        await collection.InsertOneAsync(record, null, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetRecordsAsync<T>(string table, CancellationToken cancellationToken)
    {
        var collection = db.GetCollection<T>(table);
        var result = await collection.FindAsync(new BsonDocument(), null, cancellationToken);
        return await result.ToListAsync(cancellationToken);
    }

    public async Task RemoveRecordAsync<T>(string table, Guid id, CancellationToken cancellationToken)
    {
        var collection = db.GetCollection<T>(table);
        var filter = Builders<T>.Filter.Eq("Id", id);

        await collection.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task UpdateRecordAsync<T>(string table, T record, Guid id, CancellationToken cancellationToken)
    {
        var collection = db.GetCollection<T>(table);
        var filter = Builders<T>.Filter.Eq("Id", id);

        await collection.ReplaceOneAsync(filter, record, new ReplaceOptions { IsUpsert = false }, cancellationToken);
    }
}