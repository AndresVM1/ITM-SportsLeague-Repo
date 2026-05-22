using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{


    // Servicio de dominio para gestionar alineaciones de partidos, con lógica de negocio específica para validar y manejar alineaciones
    public interface IMatchLineupService
    {
        Task<MatchLineup> AddPlayerAsync(int matchId, MatchLineup lineup);

        Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId);

        Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId);

        Task DeleteAsync(int matchId, int lineupId);
    }

}