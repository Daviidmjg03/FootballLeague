using ProjectLigaNosWeb.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Data
{
    public interface IClubsRepository : IGenericRepository<Clubs>
    {
        public IQueryable GetAllWithUsers();

        Task<List<Clubs>> GetAllAsync();
        Task<Clubs> GetByIdAsync(int id);
        Task CreateAsync(Clubs club);
        Task UpdateAsync(Clubs club);
        Task DeleteAsync(Clubs club);

    }
}
