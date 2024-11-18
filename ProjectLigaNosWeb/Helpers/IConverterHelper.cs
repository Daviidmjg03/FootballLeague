using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Models;
using System;

namespace ProjectLigaNosWeb.Helpers
{
    public interface IConverterHelper
    {
        Clubs ToClubs(ClubViewModel model, Guid imageId, bool isNew);

        ClubViewModel ToClubeViewModel(Clubs clubes);
    }
}
