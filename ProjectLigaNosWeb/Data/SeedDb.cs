using Microsoft.AspNetCore.Identity;
using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {

            _context = context;
            _userHelper = userHelper;
            _random = new Random();
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Funcionario");
            await _userHelper.CheckRoleAsync("Clube");

            var user = await _userHelper.GetUserByEmailAsync("davidmjgjogos@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    FirstName = "David",
                    LastName = "Gonçalves",
                    Email = "davidmjgjogos@gmail.com",
                    UserName = "davidmjgjogos@gmail.com",
                    PhoneNumber = "1234567890",
                    Address = "dadada",
                    PostalCode = "12345"
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");
                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");
            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            if (!_context.Clubs.Any())
            {
                AddClubs("Benfica", "SLB", user);
                AddClubs("Porto", "FCP", user);
                AddClubs("Sporting", "SCP", user);
                AddClubs("Boavista", "BOA", user);

                await _context.SaveChangesAsync();
            }
        }

        private void AddClubs(string nome, string acroyn, User user)
        {
            _context.Clubs.Add(new Clubs
            {
                Name = nome,
                Acroyn = acroyn,
                DateFund = 1904, 
                City = "Lisboa", 
                Country = "Portugal",
                CapacityStadium = 50000, 
                President = "President A", 
                NationalTitles = 0,
                InternationalTitles = 0, 
                User = user
            });
        }
    }
}
