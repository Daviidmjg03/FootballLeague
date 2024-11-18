using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace ProjectLigaNosWeb.Data.Entities
{
    public class Clubs : IEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "The field {0} cannot contain more than {1} characters.")]
        public string Name { get; set; }

        [Required]
        public string Acroyn { get; set; }

        [Required]
        public int DateFund { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        [Required]
        public string City { get; set; } 

        [Required]
        public string Country { get; set; } 

        [Required]
        public int CapacityStadium { get; set; } 

        [Required]
        public string President { get; set; }  

        [Required]
        public int NationalTitles { get; set; }  

        [Required]
        public int InternationalTitles { get; set; } 

        public User User { get; set; }

        public List<Players> Players { get; set; }

        public List<Games> HomeGames { get; set; }
        public List<Games> AwayGames { get; set; }

        public List<Statistics> Statistics { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? "/images/noimage.png"
            : $"https://supershopdg.blob.core.windows.net/clubs/{ImageId}";
    }
}
