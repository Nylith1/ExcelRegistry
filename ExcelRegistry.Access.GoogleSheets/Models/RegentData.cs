using Newtonsoft.Json;

namespace ExcelRegistry.Access.GoogleSheets.Models;

public record RegentData
{
    [JsonProperty("Id")]
    public required string Id { get; init; }

    [JsonProperty("Eilės numeris")]
    public required string IndexNumber { get; init; }

    [JsonProperty("Data")]
    public required string Date { get; init; }

    [JsonProperty("Ruošiamas tirpalas")]
    public required string SolutionBeingPrepared { get; init; }

    [JsonProperty("Pagaminimo data")]
    public required string DateOfManufacture { get; init; }

    [JsonProperty("Galiojimo data")]
    public required string ExpireDate { get; init; }

    [JsonProperty("Įrašą atlikęs asmuo")]
    public required string User { get; init; }
}
