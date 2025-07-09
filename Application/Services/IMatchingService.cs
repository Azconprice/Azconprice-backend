using Application.Models;
using Application.Models.DTOs;

namespace Application.Services
{
    public interface IMatchingService
    {
        List<MatchResultDto> FindMatches(QueryRowDto query,
                                         IReadOnlyCollection<MasterRow> masterRows);
    }
}
