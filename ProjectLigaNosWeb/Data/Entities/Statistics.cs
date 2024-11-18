using System.ComponentModel.DataAnnotations;

namespace ProjectLigaNosWeb.Data.Entities
{
    public class Statistics : IEntity
    {
        public int Id { get; set; }

        public int ClubId { get; set; }
        public Clubs Club { get; set; } 

        public int GameId { get; set; }
        public Games Game { get; set; } 

        [Required]
        public int MatchesPlayed { get; set; } 

        [Required]
        public int OverallRanking { get; set; } 

        [Required]
        public int GoalsScored { get; set; } 

        [Required]
        public int GoalsConceded { get; set; } 

        [Required]
        public int HomeWins { get; set; } 

        [Required]
        public int AwayWins { get; set; } 

        public User User { get; set; }
    }
}
