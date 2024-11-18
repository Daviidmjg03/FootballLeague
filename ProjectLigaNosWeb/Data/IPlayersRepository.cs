using ProjectLigaNosWeb.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectLigaNosWeb.Data
{
    public interface IPlayersRepository : IGenericRepository<Players>
    {
        Task<List<Players>> GetAllAsync(); 

    }
}
