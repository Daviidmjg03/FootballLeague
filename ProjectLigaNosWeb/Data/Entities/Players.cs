using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ProjectLigaNosWeb.Data.Entities
{
    public class Players : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public DateTime DateBirth { get; set; } 

        [Required]
        public string Position { get; set; }

        [Required]
        public int ShirtNum { get; set; }

        [Required]
        public string Nacionality { get; set; } 

        public int ClubId { get; set; }
        public Clubs Club { get; set; }

        public List<Statistics> Statistics { get; set; }

        public User User { get; set; }

        public string ProfilePicture { get; set; }

        public string ProfilePicturePath { get; set; }

    }
}
