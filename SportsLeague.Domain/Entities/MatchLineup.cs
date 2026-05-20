using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Entities
{
    public class MatchLineup : AuditBase
    {
        public int MatchId { get; set; } // Foreign key to Match
        public int PlayerId { get; set; } // Foreign key to Player

        public bool IsStarting { get; set; } // Indicates if the player is in the starting lineup

        public PlayerPosition Position { get; set; } // Position of the player in the lineup (e.g., Goalkeeper, Defender, Midfielder, Forward)


        // Navigation properties

        public Player Player { get; set; } = null!;

        public Match Match { get; set; } = null!;


    }
}
