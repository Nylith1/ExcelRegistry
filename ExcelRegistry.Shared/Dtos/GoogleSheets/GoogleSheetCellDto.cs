namespace ExcelRegistry.Shared.Dtos.GoogleSheets;

public record GoogleSheetCellDto
{
    public string StringCellValue { get; init; } = string.Empty;

    public bool IsBold { get; init; }

    public double? CurrencyCellValue { get; init; }

    public bool IsCurrency { get; init; }

    public string CurrencyType { get; init; } = string.Empty;

    public bool IsNumber { get; init; }

    public double? NumberCellValue { get; init; }
}
