using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectLigaNosWeb.Models
{
    public class PlayerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The player's name is required.")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The player's date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DateBirth { get; set; }

        [Required(ErrorMessage = "The player's position is required.")]
        public string Position { get; set; }

        [Required(ErrorMessage = "The shirt number is required.")]
        public int ShirtNum { get; set; }

        [Required(ErrorMessage = "The nationality is required.")]
        public string Nacionality { get; set; }

        [Required(ErrorMessage = "The club is required.")]
        public int ClubId { get; set; }

        public string ClubName { get; set; }

        public List<SelectListItem> Clubs { get; set; }

        public IFormFile ProfilePicture { get; set; }

        public string ProfilePicturePath { get; set; }

        public PlayerViewModel()
        {
            DateBirth = new DateTime(2024, 1, 1);
        }
    }
}
