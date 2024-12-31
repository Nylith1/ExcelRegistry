using ExcelRegistry.Domain.Handlers.Repositories.Regent;
using ExcelRegistry.Domain.Regents;
using ExcelRegistry.Shared.Dtos.GoogleSheets;
using MediatR;

namespace ExcelRegistry.Domain.Handlers.Regents;

public class AddRegentRequestHandler(IRegentRepository regentRepository) : IRequestHandler<AddRegentRequest>
{
    public async Task Handle(AddRegentRequest request, CancellationToken cancellationToken)
    {
        var lastRowNumber = await regentRepository.GetLastRegentRowIndex(cancellationToken);
        await regentRepository.AddRegent(CreateRegent(request, lastRowNumber).MapToDto(), cancellationToken);
    }

    private static Regent CreateRegent(AddRegentRequest request, int rowNumber) => Regent.Create(new RegentDto
    {
        Id = request.Id,
        Date = request.Date,
        DateOfManufacture = request.DateOfManufacture,
        ExpireDate = request.ExpireDate,
        RowNumber = rowNumber.ToString(),
        SolutionBeingPrepared = request.SolutionBeingPrepared,
        UserName = request.User,
    }, request.TimeZoneOffsetInHours);
}

public record AddRegentRequest : IRequest
{
    public required string Id { get; init; }

    public required DateTime Date { get; init; }

    public required string SolutionBeingPrepared { get; init; }

    public required DateTime DateOfManufacture { get; init; }

    public required DateTime ExpireDate { get; init; }

    public required string User { get; init; }

    public required int TimeZoneOffsetInHours { get; init; }
}
