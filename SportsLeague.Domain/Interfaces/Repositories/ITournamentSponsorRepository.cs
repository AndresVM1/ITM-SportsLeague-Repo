using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ITournamentSponsorRepository
{
    Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId);

    Task<IEnumerable<TournamentSponsor>> GetByTournamentAsync(int tournamentId);
}
