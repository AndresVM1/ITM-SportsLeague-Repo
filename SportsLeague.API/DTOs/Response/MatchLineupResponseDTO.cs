

using SportsLeague.Domain.Enums;

namespace SportsLeague.API.Dtos.MatchLineup;

public class MatchLineupDto
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public bool IsStarting { get; set; }
    public PlayerPosition Position { get; set; }
}