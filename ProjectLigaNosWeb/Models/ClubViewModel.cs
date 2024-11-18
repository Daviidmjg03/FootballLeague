using Microsoft.AspNetCore.Http;
using ProjectLigaNosWeb.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProjectLigaNosWeb.Models
{
    public class ClubViewModel : Clubs
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

    }
}
