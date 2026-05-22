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


    // Metodo para obtener alineación por partido y equipo, incluyendo detalles del jugador y su equipo
    public async Task<List<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
    {
        return await _context.MatchLineups
            .Include(ml => ml.Player)
            .Where(ml => ml.MatchId == matchId && ml.Player.TeamId == teamId)
            .ToListAsync();
    }


    // Metodo para verificar si un jugador ya está registrado en la alineación de un partido específico, evitando duplicados
    public async Task<bool> ExistsByMatchAndPlayerAsync(int matchId, int playerId)
    {
        return await _context.MatchLineups
            .AnyAsync(ml => ml.MatchId == matchId && ml.PlayerId == playerId);
    }


    // Metodo para contar el número de titulares registrados para un equipo en un partido específico, asegurando que no se exceda el límite de 11 titulares
    public async Task<int> CountStartersByTeamAsync(int matchId, int teamId)
    {
        return await _context.MatchLineups
            .CountAsync(ml => ml.MatchId == matchId &&
                             ml.Player.TeamId == teamId &&
                             ml.IsStarting);
    }
}