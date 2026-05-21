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
            // V1 + V6: Validar partido (existe y está Scheduled)
            var match = await _validationHelper.ValidateLineupMatchAsync(matchId);

            // V2 + V3: Validar jugador (existe y pertenece al partido)
            var player = await _validationHelper.ValidatePlayerInMatchAsync(lineup.PlayerId, match);

            // V4: No duplicado
            if (await _lineupRepository.ExistsByMatchAndPlayerAsync(matchId, lineup.PlayerId))
                throw new InvalidOperationException("El jugador ya está registrado en la alineación de este partido");

            // V5: Máximo 11 titulares
            if (lineup.IsStarting)
            {
                int startersCount = await _lineupRepository.CountStartersByTeamAsync(matchId, player.TeamId);
                if (startersCount >= 11)
                    throw new InvalidOperationException("El equipo ya tiene 11 titulares registrados en este partido");
            }

            lineup.MatchId = matchId;

            _logger.LogInformation("Adding player {PlayerId} to match {MatchId} lineup",
                lineup.PlayerId, matchId);

            return await _lineupRepository.CreateAsync(lineup);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAsync(int matchId)
        {
            // Validamos que el partido exista
            await _validationHelper.ValidateLineupMatchAsync(matchId);

            return await _lineupRepository.GetByMatchAsync(matchId);
        }

        public async Task<IEnumerable<MatchLineup>> GetByMatchAndTeamAsync(int matchId, int teamId)
        {
            return await _lineupRepository.GetByMatchAndTeamAsync(matchId, teamId);
        }

        public async Task DeleteAsync(int matchId, int lineupId)
        {
            // Validar partido
            await _validationHelper.ValidateLineupMatchAsync(matchId);

            var existingLineup = await _lineupRepository.GetByIdAsync(lineupId);
            if (existingLineup == null)
                throw new KeyNotFoundException($"No se encontró el registro de alineación con ID {lineupId}");

            if (existingLineup.MatchId != matchId)
                throw new InvalidOperationException("El registro de alineación no corresponde al partido especificado");

            _logger.LogInformation("Deleting lineup record {LineupId} from match {MatchId}", lineupId, matchId);

            await _lineupRepository.DeleteAsync(lineupId);
        }
    }
}