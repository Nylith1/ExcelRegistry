using ExcelRegistry.Shared.Dtos.GoogleSheets;

namespace ExcelRegistry.Domain.Handlers.Repositories.Regent;

public interface IRegentRepository
{
    Task AddRegent(RegentDto regent, CancellationToken cancellationToken);
    Task EditRegent(string Id, RegentDto regent, CancellationToken cancellationToken);
    Task<int> GetLastRegentRowIndex(CancellationToken cancellationToken);
    Task<IEnumerable<RegentDto>> GetRegents(CancellationToken cancellationToken);
}