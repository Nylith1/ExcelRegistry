using ExcelRegistry.Access.GoogleSheets.Repositories.Interfaces;
using ExcelRegistry.Shared.Dtos.GoogleSheets;
using MediatR;

namespace ExcelRegistry.Domain.Handlers.Regents;

public class GetRegentsRequestHandler(IRegentRepository regentRepository) : IRequestHandler<GetRegentsRequest, IEnumerable<GetRegentResponseItem>>
{
    public async Task<IEnumerable<GetRegentResponseItem>> Handle(GetRegentsRequest request, CancellationToken cancellationToken)
    {
        var regentsData = await regentRepository.GetRegents(cancellationToken);
        return MapToResponse(regentsData);
    }

    private static IEnumerable<GetRegentResponseItem> MapToResponse(IEnumerable<RegentDto> regentsData) => regentsData.Select(x => new GetRegentResponseItem
    {
        Id = x.Id,
        Date = x.Date,
        DateOfManufacture = x.DateOfManufacture,
        ExpireDate = x.ExpireDate,
        IndexNumber = x.RowNumber,
        SolutionBeingPrepared = x.SolutionBeingPrepared,
        User = x.UserName
    });
}

public record GetRegentsRequest : IRequest<IEnumerable<GetRegentResponseItem>>
{
}

public record GetRegentResponseItem
{
    public required string Id { get; init; }

    public required string IndexNumber { get; init; }

    public required DateTime Date { get; init; }

    public required string SolutionBeingPrepared { get; init; }

    public required DateTime DateOfManufacture { get; init; }

    public required DateTime ExpireDate { get; init; }

    public required string User { get; init; }
}
