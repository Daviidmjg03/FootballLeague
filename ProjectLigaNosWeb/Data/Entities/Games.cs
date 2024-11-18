using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ProjectLigaNosWeb.Data.Entities
{
    public class Games : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]

        public DateTime Data { get; set; }

        public int ClubHomeId { get; set; }
        public Clubs ClubHome { get; set; }

        public int ClubAwayId { get; set; }
        public Clubs ClubAway { get; set; }

        [Required]
        public int HomeGoals { get; set; }
        [Required]
        public int AwayGoals { get; set; }
        [Required]
        public string Localizacao { get; set; }

        public User User { get; set; }
        public List<Statistics> Statistics { get; set; } = new List<Statistics>();

    }
}
