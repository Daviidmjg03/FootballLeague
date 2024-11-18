using Microsoft.EntityFrameworkCore;
using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Data
{
    public class ClubsRepository : GenericRepository<Clubs>, IClubsRepository
    {
        private readonly DataContext _context;
        public ClubsRepository(DataContext context) :base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            return _context.Clubs.Include(c => c.User);
        }

        public async Task<List<Clubs>> GetAllAsync()
        {
            return await _context.Clubs.ToListAsync();
        }

        public async Task AddClubAsync(Clubs club)
        {
            await _context.Clubs.AddAsync(club);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClubAsync(Clubs club)
        {
            _context.Clubs.Update(club);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteClubAsync(int clubId)
        {
            var club = await _context.Clubs
                .Include(c => c.Players)
                .Include(c => c.HomeGames)
                .Include(c => c.AwayGames)
                .Include(c => c.Statistics)
                .FirstOrDefaultAsync(c => c.Id == clubId);

            if (club == null)
            {
                return false; 
            }

            if (club.Players.Any() || club.HomeGames.Any() || club.AwayGames.Any() || club.Statistics.Any())
            {
                return false; 
            }

            _context.Clubs.Remove(club);
            await _context.SaveChangesAsync();
            return true; 
        }

        public async Task<Clubs> GetByIdAsync(int id)
        {
            return await _context.Clubs.FindAsync(id);
        }
    }

}
