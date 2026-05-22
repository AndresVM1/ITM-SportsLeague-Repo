using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;



namespace SportsLeague.API.DTOs.Request


{
    public class MatchLineupRequestDTO
    {
        public int PlayerId { get; set; }
        public bool IsStarting { get; set; }
        public PlayerPosition Position { get; set; }

    }
}
