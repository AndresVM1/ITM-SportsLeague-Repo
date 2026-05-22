using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories
{
    public class MatchLineupRepository : GenericRepository<MatchLineup>, IMatchLineupRepository
    {
        public MatchLineupRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            return await _dbSet
                .Include(ml => ml.Player)
                    .ThenInclude(p => p.Team)
                .Where(ml => ml.MatchId == matchId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _dbSet
                .Include(ml => ml.Player)
                .Where(ml => ml.MatchId == matchId && ml.Player.TeamId == teamId)
                .ToListAsync();
        }

        public async Task<bool> ExistsByMatchAndPlayerAsync(int matchId, int playerId)
        {
            return await _dbSet.AnyAsync(ml => ml.MatchId == matchId && ml.PlayerId == playerId);
        }

        public async Task<int> CountStartersByTeamAsync(int matchId, int teamId)
        {
            // Contamos solo los jugadores titulares registrados para el equipo en el partido
            return await _dbSet.CountAsync(ml => ml.MatchId == matchId && ml.Player.TeamId == teamId && ml.IsStarting == true);
        }
    }
}