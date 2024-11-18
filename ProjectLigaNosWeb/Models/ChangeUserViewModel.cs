using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ProjectLigaNosWeb.Models
{
    public class ChangeUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } 

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; } 

        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } 

        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }

        public string ProfilePicturePath { get; set; }
    }
}
