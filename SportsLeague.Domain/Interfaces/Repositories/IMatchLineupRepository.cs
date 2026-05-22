using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;

// Repositorio específico para MatchLineup, con métodos personalizados para consultas relacionadas con alineaciones de partidos
public interface IMatchLineupRepository : IGenericRepository<MatchLineup>
{
    Task<List<MatchLineup>> GetByMatchAsync(int matchId);
    Task<List<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId);
    Task<bool> ExistsByMatchAndPlayerAsync(int matchId, int playerId);
    Task<int> CountStartersByTeamAsync(int matchId, int teamId);
}