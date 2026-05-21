using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;



namespace SportsLeague.API.DTOs.Request


{
    public class MatchLineupRequestDTO
    {
        public PlayerPosition Position { get; set; }
        public int PlayerId { get; set; }
        public bool IsStarting { get; set; }

    }
}
