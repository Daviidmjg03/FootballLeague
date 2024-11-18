using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectLigaNosWeb.Data
{
    public class PlayersRepository : GenericRepository<Players>, IPlayersRepository
    {
        private readonly DataContext _context;

        public PlayersRepository(DataContext context) : base(context)
        {
            _context = context;

        }

        public async Task<List<Players>> GetAllAsync()
        {
            return await _context.Players.Include(p => p.Club).ToListAsync();
        }
    }
}
