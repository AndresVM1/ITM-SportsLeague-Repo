using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;

namespace SportsLeague.DataAccess.Repositories;

public class SponsorRepositoy : GenericRepository<Sponsor>, ISponsorRepository
{
    public SponsorRepositoy(LeagueDbContext context) : base(context)
    {
    }

    public async Task<Sponsor?> ExistByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
    }
}
