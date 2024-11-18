using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Data
{
    public class GamesRepository : GenericRepository<Games>, IGamesRepository
    {
        private readonly DataContext _context;

        public GamesRepository(DataContext context) : base(context)
        {
            _context = context;

        }

        public async Task<List<Games>> GetAllAsync()
        {
            return await _context.Games
                .Include(g => g.ClubHome)
                .Include(g => g.ClubAway) 
                .ToListAsync();
        }

        public async Task<Games> GetByIdAsync(int id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task CreateAsync(Games game)
        {
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Games game)
        {
            _context.Games.Update(game); 
            await _context.SaveChangesAsync(); 
        }

        public async Task DeleteAsync(Games game)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }
    }
}
