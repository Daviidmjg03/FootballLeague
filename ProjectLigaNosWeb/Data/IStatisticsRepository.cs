using ProjectLigaNosWeb.Data.Entities;
using ProjectLigaNosWeb.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Data
{
    public interface IStatisticsRepository : IGenericRepository<Statistics>
    {
        Task<List<Statistics>> GetAllAsync();
        Task<Statistics> GetByIdAsync(int id);
        Task AddAsync(Statistics statistics); 
        Task UpdateAsync(Statistics statistics);
        Task DeleteAsync(Statistics statistics);
        Task<bool> ExistsAsync(int id);
    }
}
