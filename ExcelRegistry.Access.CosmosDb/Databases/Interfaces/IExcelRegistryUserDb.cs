namespace ExcelRegistry.Access.CosmosDb.Databases.Interfaces;

public interface IExcelRegistryUserDb
{
    Task<IEnumerable<T>> GetRecordsAsync<T>(string table, CancellationToken cancellationToken);
    Task InsertRecordsAsync<T>(string table, T record, CancellationToken cancellationToken);
    Task RemoveRecordAsync<T>(string table, Guid id, CancellationToken cancellationToken);
    Task UpdateRecordAsync<T>(string table, T record, Guid id, CancellationToken cancellationToken);
}
