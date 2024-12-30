using ExcelRegistry.Access.GoogleSheets.Models;
using ExcelRegistry.Access.GoogleSheets.Repositories.Interfaces;
using ExcelRegistry.Access.GoogleSheets.Services.Interfaces;
using ExcelRegistry.Shared;
using ExcelRegistry.Shared.Dtos.GoogleSheets;
using Newtonsoft.Json;
using System.Dynamic;

namespace ExcelRegistry.Access.GoogleSheets.Repositories;

public class RegentRepository(IGoogleSheetsService googleSheetsService) : IRegentRepository
{
    public async Task<IEnumerable<RegentDto>> GetRegents(CancellationToken cancellationToken)
    {
        var sheetData = await googleSheetsService.GetDataFromSheetAsync(new() { RangeColumnEnd = Constants.RegentSheet.RageColumnEndForRegents, SheetName = Constants.RegentSheet.SheetName }, cancellationToken);
        return MapToDto(sheetData);
    }

    public async Task EditRegent(string Id, RegentDto regent, CancellationToken cancellationToken)
    {
        await googleSheetsService.UpdateRowAsync(Constants.RegentSheet.SheetName, Id, MapToRowDto(regent), cancellationToken);
    }

    public async Task AddRegent(RegentDto regent, CancellationToken cancellationToken)
    {
        await googleSheetsService.AddNextRowAsync(MapToRowDto(regent), Constants.RegentSheet.SheetName, cancellationToken);
    }

    public async Task<int> GetLastRegentRowIndex(CancellationToken cancellationToken)
    {
        return await googleSheetsService.GetLastRowIndexAsync(Constants.RegentSheet.SheetName, Constants.RegentSheet.RageColumnEndForRegents, cancellationToken);
    }

    private static List<RegentDto> MapToDto(List<ExpandoObject> sheetData)
    {
        var registeredRegents = new List<RegentDto>();
        foreach (var item in sheetData)
        {
            var regent = JsonConvert.DeserializeObject<RegentData>(JsonConvert.SerializeObject(item)) ?? throw new Exception($"DeserializeObject {nameof(RegentData)} failed.");
            registeredRegents.Add(new()
            {
                Id = regent.Id,
                RowNumber = regent.IndexNumber,
                Date = DateTime.Parse(regent.Date),
                SolutionBeingPrepared = regent.SolutionBeingPrepared,
                DateOfManufacture = DateTime.Parse(regent.DateOfManufacture),
                ExpireDate = DateTime.Parse(regent.ExpireDate),
                UserName = regent.User
            });
        }

        return registeredRegents;
    }

    private static GoogleSheetRowDto MapToRowDto(RegentDto regent) => new()
    {
        Cells =
        [
            new() { StringCellValue = regent.Id },
            new() { StringCellValue = regent.RowNumber },
            new() { StringCellValue = regent.Date.ToString() },
            new() { StringCellValue = regent.SolutionBeingPrepared },
            new() { StringCellValue = regent.DateOfManufacture.ToShortDateString() },
            new() { StringCellValue = regent.ExpireDate.ToShortDateString() },
            new() { StringCellValue = regent.UserName }
        ]
    };
}
