using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces
{
    public interface IGameRepository
    {
        Task<Game?> GetByIdAsync(int id);
        Task<IEnumerable<Game>> GetAllAsync();
        Task AddAsync(Game game);
        Task UpdateAsync(Game game);
        Task DeleteAsync(Game game);
    }
}
