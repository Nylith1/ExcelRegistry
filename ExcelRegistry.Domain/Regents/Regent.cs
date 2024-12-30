using ExcelRegistry.Domain.Common;
using ExcelRegistry.Shared.Dtos.GoogleSheets;
using ExcelRegistry.Shared.Helpers.Exceptions;

namespace ExcelRegistry.Domain.Regents;

public class Regent
{
    private Regent(RegentDomainDto domainDto)
    {
        Id = domainDto.Id;
        Date = domainDto.Date;
        ExpireDate = domainDto.ExpireDate;
        DateOfManufacture = domainDto.DateOfManufacture;
        RowNumber = domainDto.RowNumber;
        SolutionBeingPrepared = domainDto.SolutionBeingPrepared;
        UserName = domainDto.UserName;
    }

    public GuidId Id { get; private set; }
    public RegentDate Date { get; private set; }
    public RegentExpireDate ExpireDate { get; private set; }
    public RegentDateOfManufacture DateOfManufacture { get; private set; }
    public RegentRowNumber RowNumber { get; private set; }
    public RegentSolutionBeingPrepared SolutionBeingPrepared { get; private set; }
    public UserName UserName { get; private set; }

    public static Regent Create(RegentDto dto, int timeZoneOffsetInHours)
    {
        var domainDto = new RegentDomainDto()
        {
            Id = GuidId.Create(dto.Id),
            Date = RegentDate.Create(dto.Date, timeZoneOffsetInHours),
            DateOfManufacture = RegentDateOfManufacture.Create(dto.DateOfManufacture, timeZoneOffsetInHours),
            ExpireDate = RegentExpireDate.Create(dto.ExpireDate, timeZoneOffsetInHours),
            RowNumber = RegentRowNumber.Create(dto.RowNumber),
            SolutionBeingPrepared = RegentSolutionBeingPrepared.Create(dto.SolutionBeingPrepared),
            UserName = UserName.Create(dto.UserName),
        };

        Validate(domainDto);
        return new Regent(domainDto);
    }

    private static void Validate(RegentDomainDto domainDto)
    {
        if (domainDto.ExpireDate.Value <= domainDto.DateOfManufacture.Value)
            throw new DomainException("Galiojimo data turi būti senesnė nei pagaminimo datos.");
    }

    public RegentDto MapToDto() => new()
    {
        Id = Id.Value,
        Date = Date.Value,
        DateOfManufacture = DateOfManufacture.Value,
        ExpireDate = ExpireDate.Value,
        RowNumber = RowNumber.Value,
        SolutionBeingPrepared = SolutionBeingPrepared.Value,
        UserName = UserName.Value,
    };
}

public record RegentDomainDto
{
    public required GuidId Id { get; init; }
    public required RegentDate Date { get; init; }
    public required RegentExpireDate ExpireDate { get; init; }
    public required RegentDateOfManufacture DateOfManufacture { get; init; }
    public required RegentRowNumber RowNumber { get; init; }
    public required RegentSolutionBeingPrepared SolutionBeingPrepared { get; init; }
    public required UserName UserName { get; init; }
}