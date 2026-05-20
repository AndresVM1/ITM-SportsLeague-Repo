using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class MatchLineupRepository : GenericRepository<MatchLineup>, IMatchLineupRepository
{
    public MatchLineupRepository(LeagueDbContext context) : base(context) { }


    // Implementación de métodos específicos para MatchLineup
    public async Task<List<MatchLineup>> GetByMatchAsync(int matchId)
    {
        return await _context.MatchLineups
            .Include(ml => ml.Player)
            .ThenInclude(p => p.Team)
            .Where(ml => ml.MatchId == matchId)
            .ToListAsync();
    }


    // Method for determining the lineup of a specific team in a game
    public async Task<List<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
    {
        return await _context.MatchLineups
            .Include(ml => ml.Player)
            .Where(ml => ml.MatchId == matchId && ml.Player.TeamId == teamId)
            .ToListAsync();
    }


    // Method for checking whether a player is in the lineup for a specific game
    public async Task<bool> ExistsByMatchAndPlayerAsync(int matchId, int playerId)
    {
        return await _context.MatchLineups
            .AnyAsync(ml => ml.MatchId == matchId && ml.PlayerId == playerId);
    }


    // Method for counting the number of starters on a team in a specific game
    public async Task<int> CountStartersByTeamAsync(int matchId, int teamId)
    {
        return await _context.MatchLineups
            .CountAsync(ml => ml.MatchId == matchId &&
                             ml.Player.TeamId == teamId &&
                             ml.IsStarting);
    }
}