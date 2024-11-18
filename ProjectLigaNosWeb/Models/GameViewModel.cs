using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace ProjectLigaNosWeb.Models
{
    public class GameViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The game name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The game date is required.")]
        public DateTime Data { get; set; } = new DateTime(2024, 1, 1);

        [Required(ErrorMessage = "The home club is required.")]
        public int ClubHomeId { get; set; }

        [Required(ErrorMessage = "The away club is required.")]
        public int ClubAwayId { get; set; }

        [Required(ErrorMessage = "The home goals are required.")]
        public int HomeGoals { get; set; }

        [Required(ErrorMessage = "The away goals are required.")]
        public int AwayGoals { get; set; }

        [Required(ErrorMessage = "The location is required.")]
        public string Localizacao { get; set; }

        public List<SelectListItem> ClubsHome { get; set; }
        public List<SelectListItem> ClubsAway { get; set; }
    }
}
