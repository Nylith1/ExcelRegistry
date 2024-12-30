using ExcelRegistry.Access.GoogleSheets.Services.Interfaces;
using ExcelRegistry.Shared;
using ExcelRegistry.Shared.Dtos.GoogleSheets;
using ExcelRegistry.Shared.Options;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Options;
using System.Dynamic;

namespace ExcelRegistry.Access.GoogleSheets.Services;

public class GoogleSheetsService(IOptions<GoogleSheetsOptions> googleSheetsOptions, ISheetsServiceFactory sheetsService) : IGoogleSheetsService
{
    private readonly GoogleSheetsOptions googleSheetsOptions = googleSheetsOptions.Value;
    private readonly SheetsService sheetsService = sheetsService.Create();

    public async Task<List<ExpandoObject>> GetDataFromSheetAsync(GoogleSheetParameters googleSheetParameters, CancellationToken cancellationToken)
    {
        var range = BuildRange(googleSheetParameters);
        var responseValues = await GetSheetValuesAsync(sheetsService, googleSheetsOptions.SpreadSheetId, range, cancellationToken);
        return ParseSheetValues(responseValues, googleSheetParameters);
    }

    public async Task AddNextRowAsync(GoogleSheetRowDto row, string sheetName, CancellationToken cancellationToken)
    {
        await ExecuteUpdateAsync(new()
        {
            ColumnIndex = Constants.GoogleSheets.DefaultRangeColumnStart,
            RowIndex = await GetCurrentRowEntryCountAsync(sheetName, cancellationToken),
            SheetId = await GetSheetIdAsync(sheetName, cancellationToken),
        }, GetProcessedRow(row), cancellationToken);
    }

    public async Task UpdateRowAsync(string sheetName, string searchId, GoogleSheetRowDto row, CancellationToken cancellationToken)
    {
        await ExecuteUpdateAsync(new()
        {
            SheetId = await GetSheetIdAsync(sheetName, cancellationToken),
            RowIndex = await GetRowIndexToUpdateAsync(sheetName, searchId, cancellationToken),
            ColumnIndex = Constants.GoogleSheets.DefaultRangeColumnStart
        }, GetProcessedRow(row), cancellationToken);
    }

    public async Task DeleteRowAsync(string sheetName, string searchValue, CancellationToken cancellationToken)
    {
        int rowIndex = await GetRowIndexToUpdateAsync(sheetName, searchValue, cancellationToken);

        var deleteRequest = new Request
        {
            DeleteDimension = new DeleteDimensionRequest
            {
                Range = new DimensionRange
                {
                    SheetId = await GetSheetIdAsync(sheetName, cancellationToken),
                    Dimension = "ROWS",
                    StartIndex = rowIndex,
                    EndIndex = rowIndex + 1
                }
            }
        };

        var batchUpdateRequest = new BatchUpdateSpreadsheetRequest
        {
            Requests = [deleteRequest]
        };

        await sheetsService.Spreadsheets.BatchUpdate(batchUpdateRequest, googleSheetsOptions.SpreadSheetId).ExecuteAsync(cancellationToken);
    }

    public async Task<int> GetLastRowIndexAsync(string sheetName, int rageColumnEndForRegents, CancellationToken cancellationToken)
    {
        var gsp = new GoogleSheetParameters { RangeColumnEnd = rageColumnEndForRegents, SheetName = sheetName };
        var gspData = await GetDataFromSheetAsync(gsp, cancellationToken);

        return gspData.Count + 1;
    }

    private static string BuildRange(GoogleSheetParameters parameters) =>
        $"{parameters.SheetName}!" +
        $"{GetColumnName(parameters.RangeColumnStart)}{parameters.RangeRowStart}:" +
        $"{GetColumnName(parameters.RangeColumnEnd)}{parameters.RangeRowEnd}";

    private static string GetColumnName(int index)
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var value = "";

        if (index >= letters.Length)
            value += letters[index / letters.Length - 1];

        value += letters[index % letters.Length];
        return value;
    }

    private static async Task<IList<IList<object>>> GetSheetValuesAsync(SheetsService sheetsService, string spreadsheetId, string range, CancellationToken cancellationToken)
    {
        var request = sheetsService.Spreadsheets.Values.Get(spreadsheetId, range);
        var response = await request.ExecuteAsync(cancellationToken);
        return response.Values ?? [];
    }

    private static List<ExpandoObject> ParseSheetValues(IList<IList<object>> values, GoogleSheetParameters parameters)
    {
        var returnValues = new List<ExpandoObject>();
        var columnNames = new List<string>();

        if (values == null || values.Count == 0) return returnValues;

        var isHeaderRowProcessed = false;
        var numberOfColumns = parameters.RangeColumnEnd - parameters.RangeColumnStart;
        foreach (var row in values)
        {
            if (parameters.FirstRowIsHeaders && !isHeaderRowProcessed)
            {
                columnNames = ParseColumnHeaders(row, numberOfColumns);
                isHeaderRowProcessed = true;
            }
            else
            {
                returnValues.Add(ParseRowToExpando(row, columnNames));
            }
        }

        return returnValues;
    }

    private static List<string> ParseColumnHeaders(IList<object> headerRow, int numberOfColumns)
    {
        var columnNames = new List<string>();

        for (var i = 0; i <= numberOfColumns; i++)
        {
            columnNames.Add(headerRow[i].ToString()!);
        }

        return columnNames;
    }

    private static ExpandoObject ParseRowToExpando(IList<object> row, IList<string> columnNames)
    {
        var expando = new ExpandoObject();
        var expandoDict = expando as IDictionary<string, object>;

        for (var i = 0; i < columnNames.Count; i++)
        {
            var columnName = columnNames[i];
            var cellValue = i < row.Count ? row[i]?.ToString() : null;
            expandoDict.Add(columnName, cellValue!);
        }

        return expando;
    }

    private async Task ExecuteUpdateAsync(GridCoordinate gridCoordinate, RowData row, CancellationToken cancellationToken)
    {
        var updateRequest = new UpdateCellsRequest { Start = gridCoordinate, Fields = "*", Rows = [row] };
        var request = new Request { UpdateCells = updateRequest };
        var batchUpdateSpreadsheetRequest = new BatchUpdateSpreadsheetRequest { Requests = [request] };
        await sheetsService.Spreadsheets.BatchUpdate(batchUpdateSpreadsheetRequest, googleSheetsOptions.SpreadSheetId).ExecuteAsync(cancellationToken);
    }

    private async Task<int> GetCurrentRowEntryCountAsync(string sheetName, CancellationToken cancellationToken)
    {
        var googleSheetParameters = new GoogleSheetParameters { SheetName = sheetName };
        var entryCount = await GetDataFromSheetAsync(googleSheetParameters, cancellationToken);
        return entryCount.Count + 1;
    }

    private async Task<int> GetSheetIdAsync(string sheetName, CancellationToken cancellationToken)
    {
        var spreadsheet = await sheetsService.Spreadsheets.Get(googleSheetsOptions.SpreadSheetId).ExecuteAsync(cancellationToken);
        var sheet = spreadsheet.Sheets.First(s => s.Properties.Title == sheetName);
        return sheet.Properties.SheetId!.Value;
    }

    private static RowData GetProcessedRow(GoogleSheetRowDto row)
    {
        var processedCells = new List<CellData>();
        foreach (var cell in row.Cells)
        {
            processedCells.Add(ProcessCell(cell));
        }

        return new() { Values = processedCells };
    }

    private static CellData ProcessCell(GoogleSheetCellDto cell)
    {
        var cellData = new CellData();
        var extendedValue = new ExtendedValue { StringValue = cell.StringCellValue };

        cellData.UserEnteredValue = extendedValue;
        var cellFormat = new CellFormat { TextFormat = new TextFormat() };

        if (cell.IsBold)
        {
            cellFormat.TextFormat.Bold = true;
        }

        if (cell.IsNumber)
        {
            extendedValue.NumberValue = cell.NumberCellValue;
            cellFormat.NumberFormat = new() { Type = "NUMBER", Pattern = $"####0.00#######" };
        }

        if (cell.IsCurrency)
        {
            extendedValue.NumberValue = cell.CurrencyCellValue;
            cellFormat.NumberFormat = new() { Type = "CURRENCY", Pattern = $"{cell.CurrencyType}## ##0.00" };
        }
        else
        {
            extendedValue.StringValue = cell.StringCellValue;
        }

        cellData.UserEnteredFormat = cellFormat;
        return cellData;
    }

    private async Task<int> GetRowIndexToUpdateAsync(string sheetName, string searchValue, CancellationToken cancellationToken)
    {
        var range = $"{sheetName}!A:Z";
        var request = sheetsService.Spreadsheets.Values.Get(googleSheetsOptions.SpreadSheetId, range);
        var response = await request.ExecuteAsync(cancellationToken);
        var values = response.Values;

        if (values == null || values.Count == 0)
        {
            throw new Exception("No data found in the sheet.");
        }

        int rowIndex = -1;

        for (int i = 0; i < values.Count; i++)
        {
            if (values[i].Count > 0 && values[i][0].ToString() == searchValue)
            {
                rowIndex = i;
                break;
            }
        }

        if (rowIndex == -1)
        {
            throw new Exception($"ID '{searchValue}' not found in the sheet.");
        }

        return rowIndex;
    }
}

public record GoogleSheetParameters
{
    public int RangeColumnStart { get; init; } = Constants.GoogleSheets.DefaultRangeColumnStart;

    public int RangeRowStart { get; init; } = Constants.GoogleSheets.DefaultRangeRowStart;

    public int RangeColumnEnd { get; init; } = Constants.GoogleSheets.DefaultRangeColumnEnd;

    public int RangeRowEnd { get; init; } = Constants.GoogleSheets.DefaultRangeRowEnd;

    public required string SheetName { get; init; }

    public bool FirstRowIsHeaders { get; init; } = true;
}




