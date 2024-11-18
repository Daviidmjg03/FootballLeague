using System.ComponentModel.DataAnnotations;

namespace ProjectLigaNosWeb.Models
{
    public class LoginViewModel
    {
   
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }
    }
}
