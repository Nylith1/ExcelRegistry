namespace ExcelRegistry.Shared.Dtos.GoogleSheets;

public record GoogleSheetRowDto
{
    public required List<GoogleSheetCellDto> Cells { get; init; }
}
