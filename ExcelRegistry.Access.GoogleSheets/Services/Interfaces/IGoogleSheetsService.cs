using ExcelRegistry.Shared.Dtos.GoogleSheets;
using System.Dynamic;

namespace ExcelRegistry.Access.GoogleSheets.Services.Interfaces;

public interface IGoogleSheetsService
{
    Task AddNextRowAsync(GoogleSheetRowDto row, string sheetName, CancellationToken cancellationToken);
    Task<List<ExpandoObject>> GetDataFromSheetAsync(GoogleSheetParameters googleSheetParameters, CancellationToken cancellationToken);
    Task DeleteRowAsync(string sheetName, string searchValue, CancellationToken cancellationToken);
    Task UpdateRowAsync(string sheetName, string searchId, GoogleSheetRowDto newValues, CancellationToken cancellationToken);
    Task<int> GetLastRowIndexAsync(string sheetName, int rageColumnEndForRegents, CancellationToken cancellationToken);
}
