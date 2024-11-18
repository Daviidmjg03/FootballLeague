using System.ComponentModel.DataAnnotations;

namespace ProjectLigaNosWeb.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
