using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectLigaNosWeb.Data
{
    public class StatisticsRepository : GenericRepository<Statistics>, IStatisticsRepository
    {
        private readonly DataContext _context;

        public StatisticsRepository(DataContext context) : base(context)
        {
            _context = context; 

        }

        public async Task AddAsync(Statistics statistics)
        {
            await _context.Statistics.AddAsync(statistics);
            await _context.SaveChangesAsync(); 

        }

        public async Task<List<Statistics>> GetAllAsync()
        {
            return await _context.Statistics
                .Include(s => s.Club)  
                .Include(s => s.Game) 
                .ToListAsync();
        }
    }
}
