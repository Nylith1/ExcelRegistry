namespace ExcelRegistry.Shared.Dtos.GoogleSheets;

public record RegentDto
{
    public required string Id { get; init; }

    public required string RowNumber { get; init; }

    public required DateTime Date { get; init; }

    public required string SolutionBeingPrepared { get; init; }

    public required DateTime DateOfManufacture { get; init; }

    public required DateTime ExpireDate { get; init; }

    public required string UserName { get; init; }
}
