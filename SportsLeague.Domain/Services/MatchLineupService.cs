using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Helpers;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class MatchLineupService : IMatchLineupService
    {
        private readonly IMatchLineupRepository _lineupRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly MatchValidationHelper _validationHelper;
        private readonly ILogger<MatchLineupService> _logger;

        public MatchLineupService(
            IMatchLineupRepository lineupRepository,
            IMatchRepository matchRepository,
            MatchValidationHelper validationHelper,
            ILogger<MatchLineupService> logger)
        {
            _lineupRepository = lineupRepository;
            _matchRepository = matchRepository;
            _validationHelper = validationHelper;
            _logger = logger;
        }

        public async Task<MatchLineup> AddPlayerAsync(int matchId, MatchLineup lineup)
        {
            // VALIDACIONES:
            // El partido debe existir y estar en 'Scheduled' para poder modificar la alineación
            var match = await _validationHelper.ValidateLineupMatchAsync(matchId);

            // Que el jugador exista y que pertenezca a un equipo del partido
            var player = await _validationHelper.ValidatePlayerInMatchAsync(lineup.PlayerId, match);

            // Que el jugador no esté ya registrado en este partido
            var isAlreadyRegistered = await _lineupRepository.ExistsByMatchAndPlayerAsync(matchId, lineup.PlayerId);
            if (isAlreadyRegistered == true)
                throw new InvalidOperationException("El jugador ya está registrado en la alineación de este partido");

            // Verificamos que hallan menos de 11 titulares registrados para el equipo 
            if (lineup.IsStarting)
            {
                int startersCount = await _lineupRepository.CountStartersByTeamAsync(matchId, player.TeamId);
                if (startersCount >= 11)
                    throw new InvalidOperationException("El equipo ya tiene 11 titulares registrados en este partido");
            }

            lineup.MatchId = matchId;

            _logger.LogInformation(
                    "Adding player {PlayerId} to match {MatchId} lineup",
                    lineup.PlayerId, matchId);
            return await _lineupRepository.CreateAsync(lineup);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            // Validamos que el partido exista
            var match = await _matchRepository.GetByIdAsync(matchId);
            if (match == null)
                throw new KeyNotFoundException($"No se encontró el partido con ID {matchId}");

            return await _lineupRepository.GetByMatchAsync(matchId);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _lineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
        }

        public async Task DeleteAsync(int matchId, int lineupId)
        {
            // VALIDACIONES:
            // El partido debe existir y estar en 'Scheduled' para poder modificar la alineación
            await _validationHelper.ValidateLineupMatchAsync(matchId);

            // El registro de alineación debe existir
            var existingLineup = await _lineupRepository.GetByIdAsync(lineupId);
            if (existingLineup == null)
                throw new KeyNotFoundException($"No se encontró el registro de alineación con ID {lineupId}");

            // Validar que el registro pertenezca efectivamente al partido de la URL
            if (existingLineup.MatchId != matchId)
                throw new InvalidOperationException("El registro de alineación no corresponde al partido especificado");

            _logger.LogInformation(
                "Deleting lineup record {LineupId} from match {MatchId}",
                lineupId, matchId);
            await _lineupRepository.DeleteAsync(lineupId);
        }
    }
}