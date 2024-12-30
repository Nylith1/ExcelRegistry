using ExcelRegistry.Shared.Dtos.GoogleSheets;

namespace ExcelRegistry.Access.GoogleSheets.Repositories.Interfaces;

public interface IRegentRepository
{
    Task AddRegent(RegentDto regent, CancellationToken cancellationToken);
    Task EditRegent(string Id, RegentDto regent, CancellationToken cancellationToken);
    Task<int> GetLastRegentRowIndex(CancellationToken cancellationToken);
    Task<IEnumerable<RegentDto>> GetRegents(CancellationToken cancellationToken);
}
