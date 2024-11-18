using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Helpers;
using ProjectLigaNosWeb.Models;
using System;
using System.IO;

namespace ProjectLigaNosWeb.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Clubs ToClubs(ClubViewModel model, Guid imageId, bool isNew)
        {
            return new Clubs
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                ImageId = imageId,
                Acroyn = model.Acroyn,
                DateFund = model.DateFund,
                City = model.City,
                Country = model.Country,
                CapacityStadium = model.CapacityStadium,
                President = model.President,
                NationalTitles = model.NationalTitles,
                InternationalTitles = model.InternationalTitles,
                User = model.User
            };
        }

        public ClubViewModel ToClubeViewModel(Clubs clubes)
        {
            return new ClubViewModel
            {
                Id = clubes.Id,
                Name = clubes.Name,
                ImageId = clubes.ImageId,
                Acroyn = clubes.Acroyn,
                DateFund = clubes.DateFund,
                City = clubes.City,
                Country = clubes.Country,
                CapacityStadium = clubes.CapacityStadium,
                President = clubes.President,
                NationalTitles = clubes.NationalTitles,
                InternationalTitles = clubes.InternationalTitles,
                User = clubes.User
            };
        }
    }
}
