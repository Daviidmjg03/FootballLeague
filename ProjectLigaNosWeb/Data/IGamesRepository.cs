using ProjectLigaNosWeb.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Data
{
    public interface IGamesRepository : IGenericRepository<Games>
    {
        Task<List<Games>> GetAllAsync();
        Task<Games> GetByIdAsync(int id);
        Task CreateAsync(Games game);
        Task UpdateAsync(Games game);
        Task DeleteAsync(Games game);

    }
}
