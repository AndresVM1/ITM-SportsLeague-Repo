using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;



namespace SportsLeague.API.DTOs.Request


{
    public class MatchLienupRequestDTO
    {
        public PlayerPosition Position { get; set; }
        public int PlayerId { get; set; }
        public bool IsStarting { get; set; }

    }
}
