using ExcelRegistry.Domain.Handlers.Repositories.Regent;
using ExcelRegistry.Domain.Regents;
using ExcelRegistry.Shared.Dtos.GoogleSheets;
using MediatR;

namespace ExcelRegistry.Domain.Handlers.Regents;

public class EditRegentRequestHandler(IRegentRepository regentRepository) : IRequestHandler<EditRegentRequest>
{
    public async Task Handle(EditRegentRequest request, CancellationToken cancellationToken)
    {
        await regentRepository.EditRegent(request.Id, CreateRegent(request).MapToDto(), cancellationToken);
    }

    private static Regent CreateRegent(EditRegentRequest request) => Regent.Create(new RegentDto
    {
        Id = request.Id,
        Date = request.Date,
        DateOfManufacture = request.DateOfManufacture,
        ExpireDate = request.ExpireDate,
        RowNumber = request.IndexNumber,
        SolutionBeingPrepared = request.SolutionBeingPrepared,
        UserName = request.User
    }, request.TimeZoneOffsetInHours);
}

public record EditRegentRequest : IRequest
{
    public required string Id { get; init; }

    public required string IndexNumber { get; init; }

    public required DateTime Date { get; init; }

    public required string SolutionBeingPrepared { get; init; }

    public required DateTime DateOfManufacture { get; init; }

    public required DateTime ExpireDate { get; init; }

    public required string User { get; init; }

    public required int TimeZoneOffsetInHours { get; init; }
}