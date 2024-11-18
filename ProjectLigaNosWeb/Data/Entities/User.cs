using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectLigaNosWeb.Data.Entities
{
    public class User : IdentityUser
    {
        [Required] 
        public string FirstName { get; set; }

        [Required] 
        public string LastName { get; set; }

        [Required] 
        public string Address { get; set; }

        [Required] 
        public string PostalCode { get; set; }

        public string ProfilePicture { get; set; } 
    }
}
