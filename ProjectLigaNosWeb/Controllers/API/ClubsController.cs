using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using ProjectLigaNosWeb.Data;

namespace ProjectLigaNosWeb.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClubsController : ControllerBase
    {
        private readonly IClubsRepository _clubRepository;

        public ClubsController(IClubsRepository clubsRepository)
        {
            _clubRepository = clubsRepository;

        }

        [HttpGet]
        public IActionResult GetClubes()
        {
            return Ok(_clubRepository.GetAllWithUsers());
        }

    }
}
